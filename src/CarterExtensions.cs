namespace Carter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class CarterExtensions
    {
        /// <summary>
        /// Adds Carter to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="options">A <see cref="CarterOptions"/> instance.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseCarter(this IApplicationBuilder builder, CarterOptions options = null)
        {
            var diagnostics = builder.ApplicationServices.GetService<CarterDiagnostics>();

            var loggerFactory = builder.ApplicationServices.GetService<ILoggerFactory>();        
            var logger = loggerFactory.CreateLogger(typeof(CarterDiagnostics));
            
            diagnostics.LogDiscoveredCarterTypes(logger);

            ApplyGlobalBeforeHook(builder, options, loggerFactory.CreateLogger("Carter.GlobalBeforeHook"));

            ApplyGlobalAfterHook(builder, options, loggerFactory.CreateLogger("Carter.GlobalAfterHook"));

            var routeBuilder = new RouteBuilder(builder);

            //Create a "startup scope" to resolve modules from
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var statusCodeHandlers = scope.ServiceProvider.GetServices<IStatusCodeHandler>();

                //Get all instances of CarterModule to fetch and register declared routes
                foreach (var module in scope.ServiceProvider.GetServices<CarterModule>())
                {
                    var moduleLogger = scope.ServiceProvider
                        .GetService<ILoggerFactory>()
                        .CreateLogger(module.GetType());
                    
                    var distinctPaths = module.Routes.Keys.Select(route => route.path).Distinct();
                    foreach (var path in distinctPaths)
                    {
                        routeBuilder.MapRoute(path, CreateRouteHandler(path, module, statusCodeHandlers, moduleLogger));
                    }
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static RequestDelegate CreateRouteHandler(
            string path, CarterModule module, IEnumerable<IStatusCodeHandler> statusCodeHandlers, ILogger logger)
        {
            return async ctx =>
            {
                if (!module.Routes.TryGetValue((ctx.Request.Method, path), out var routeHandler))
                {
                    // if the path was registered but a handler matching the
                    // current method was not found, return MethodNotFound
                    ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
                }

                // begin handling the request
                if (HttpMethods.IsHead(ctx.Request.Method))
                {
                    //Cannot read the default stream once WriteAsync has been called on it
                    ctx.Response.Body = new MemoryStream();
                }

                // run the module handlers
                bool shouldContinue = true;

                if (module.Before != null)
                {
                    shouldContinue = await module.Before(ctx);
                }

                if (shouldContinue)
                {
                    // run the route handler
                    logger.LogDebug("Executing module route handler for {Method} /{Path}", ctx.Request.Method, path);
                    await routeHandler(ctx);

                    // run after handler
                    if (module.After != null)
                    {
                        await module.After(ctx);
                    }
                }

                // run status code handler
                var scHandler = statusCodeHandlers.FirstOrDefault(x => x.CanHandle(ctx.Response.StatusCode));
                if (scHandler != null)
                {
                    await scHandler.Handle(ctx);
                }

                if (HttpMethods.IsHead(ctx.Request.Method))
                {
                    var length = ctx.Response.Body.Length;
                    ctx.Response.Body.SetLength(0);
                    ctx.Response.ContentLength = length;
                }
            };
        }

        private static void ApplyGlobalAfterHook(IApplicationBuilder builder, CarterOptions options, ILogger logger)
        {
            if (options?.After != null)
            {
                builder.Use(async (ctx, next) =>
                {
                    await next();
                    logger.LogDebug("Executing global after hook");
                    await options.After(ctx);
                });
            }
        }

        private static void ApplyGlobalBeforeHook(IApplicationBuilder builder, CarterOptions options, ILogger logger)
        {
            if (options?.Before != null)
            {
                builder.Use(async (ctx, next) =>
                {
                    logger.LogDebug("Executing global before hook");

                    var carryOn = await options.Before(ctx);
                    if (carryOn)
                    {
                        logger.LogDebug("Executing next handler after global before hook");
                        await next();
                    }
                });
            }
        }

        /// <summary>
        /// Adds Carter to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Carter to.</param>
        public static void AddCarter(this IServiceCollection services)
        {
            var assemblyCatalog = new DependencyContextAssemblyCatalog();

            var assemblies = assemblyCatalog.GetAssemblies();

            CarterDiagnostics diagnostics = new CarterDiagnostics();
            services.AddSingleton(diagnostics);

            var validators = assemblies.SelectMany(ass => ass.GetTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);

            foreach (var validator in validators)
            {
                diagnostics.AddValidator(validator);
                services.AddSingleton(typeof(IValidator), validator);
            }

            services.AddSingleton<IValidatorLocator, DefaultValidatorLocator>();

            services.AddRouting();

            var modules = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(CarterModule).IsAssignableFrom(t) &&
                    t != typeof(CarterModule) &&
                    t.IsPublic
                ));

            foreach (var module in modules)
            {
                diagnostics.AddModule(module);
                services.AddScoped(module);
                services.AddScoped(typeof(CarterModule), module);
            }

            var schs = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler)));
            foreach (var sch in schs)
            {
                diagnostics.AddStatusCodeHandler(sch);
                services.AddScoped(typeof(IStatusCodeHandler), sch);
            }

            var responseNegotiators = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(IResponseNegotiator).IsAssignableFrom(t) &&
                    t != typeof(IResponseNegotiator) &&
                    t != typeof(DefaultJsonResponseNegotiator)
                ));

            foreach (var negotiatator in responseNegotiators)
            {
                diagnostics.AddResponseNegotiator(negotiatator);
                services.AddSingleton(typeof(IResponseNegotiator), negotiatator);
            }

            services.AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>();
        }
    }
}
