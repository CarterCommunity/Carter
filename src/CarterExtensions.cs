namespace Carter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using static OpenApi.CarterOpenApi;

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

            var routeMetaData = new Dictionary<(string verb, string path), RouteMetaData>();

            //Create a "startup scope" to resolve modules from so that they're cleaned up post-startup
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var statusCodeHandlers = scope.ServiceProvider.GetServices<IStatusCodeHandler>().ToList();

                //Get all instances of CarterModule to fetch and register declared routes
                foreach (var module in scope.ServiceProvider.GetServices<CarterModule>())
                {
                    var moduleLogger = scope.ServiceProvider
                        .GetService<ILoggerFactory>()
                        .CreateLogger(module.GetType());

                    routeMetaData = routeMetaData.Concat(module.RouteMetaData).ToDictionary(x => x.Key, x => x.Value);

                    var distinctPaths = module.Routes.Keys.Select(route => route.path).Distinct();
                    foreach (var path in distinctPaths)
                    {
                        routeBuilder.MapRoute(path, CreateRouteHandler(path, module.GetType(), statusCodeHandlers, moduleLogger));
                    }
                }
            }

            routeBuilder.MapRoute("openapi", BuildOpenApiResponse(options, routeMetaData));

            return builder.UseRouter(routeBuilder.Build());
        }

        private static RequestDelegate CreateRouteHandler(
            string path, Type moduleType, IEnumerable<IStatusCodeHandler> statusCodeHandlers, ILogger logger)
        {
            return async ctx =>
            {
                // Now in per-request scope
                var module = ctx.RequestServices.GetRequiredService(moduleType) as CarterModule;

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
                    foreach (var beforeDelegate in module.Before.GetInvocationList())
                    {
                        var beforeTask = (Task<bool>)beforeDelegate.DynamicInvoke(ctx);
                        shouldContinue = await beforeTask;
                        if (!shouldContinue)
                        {
                            break;
                        }
                    }
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

        public static void AddCarter(this IServiceCollection services, CarterConfigurator configurator)
        {
            AddCarter(services, 
                configurator.moduleTypes, 
                configurator.validatorTypes,
                configurator.statusCodeHandlerTypes,
                configurator.responseNegotiatorTypes);
        }

        public static void AddCarter(this IServiceCollection services,
            DependencyContextAssemblyCatalog assemblyCatalog = null)
        {
            assemblyCatalog = assemblyCatalog ?? new DependencyContextAssemblyCatalog();

            var assemblies = assemblyCatalog.GetAssemblies();

            var validators = assemblies.SelectMany(ass => ass.GetTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);

            var modules = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(CarterModule).IsAssignableFrom(t) &&
                    t != typeof(CarterModule) &&
                    t.IsPublic
                ));

            var schs = assemblies.SelectMany(x =>
                x.GetTypes().Where(t =>
                    typeof(IStatusCodeHandler).IsAssignableFrom(t) && 
                    t != typeof(IStatusCodeHandler)));
            
            var responseNegotiators = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(IResponseNegotiator).IsAssignableFrom(t) &&
                    t != typeof(IResponseNegotiator) &&
                    t != typeof(DefaultJsonResponseNegotiator)
                ));
            
            AddCarter(services, modules, validators, schs, responseNegotiators);
        }
        
        /// <summary>
        /// Adds Carter to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Carter to.</param>
        public static void AddCarter(this IServiceCollection services, IEnumerable<Type> moduleTypes, IEnumerable<Type> validatorTypes,
            IEnumerable<Type> statusCodeHandlerTypes, IEnumerable<Type> responseNegotiators)
        {
            CarterDiagnostics diagnostics = new CarterDiagnostics();
            services.AddSingleton(diagnostics);

            foreach (var validator in validatorTypes)
            {
                diagnostics.AddValidator(validator);
                services.AddSingleton(typeof(IValidator), validator);
            }

            services.AddSingleton<IValidatorLocator, DefaultValidatorLocator>();

            services.AddRouting();

            foreach (var module in moduleTypes)
            {
                diagnostics.AddModule(module);
                services.AddScoped(module);
                services.AddScoped(typeof(CarterModule), module);
            }
            
            foreach (var sch in statusCodeHandlerTypes)
            {
                diagnostics.AddStatusCodeHandler(sch);
                services.AddScoped(typeof(IStatusCodeHandler), sch);
            }

            foreach (var negotiator in responseNegotiators)
            {
                diagnostics.AddResponseNegotiator(negotiator);
                services.AddSingleton(typeof(IResponseNegotiator), negotiator);
            }

            services.AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>();
        }
    }

    public static class TypeExtensions
    {
        public static void MustDeriveFrom<T>(this Type[] types)
        {
            var invalidTypes = types.Where(m => !typeof(T).IsAssignableFrom(m)).ToList();
            if(invalidTypes.Any()) 
                throw new ArgumentException($"Modules must derive from {typeof(T).Name}, failing types:" +
                    $"{string.Join(",", invalidTypes)}");
        }
    }
}
