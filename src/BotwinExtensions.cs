namespace Botwin
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class BotwinExtensions
    {
        private static ILoggerFactory BotwinLoggerFactory { get; set; }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="options">A <see cref="BotwinOptions"/> instance.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> to be passed for tracing of Botwin setup</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder, BotwinOptions options = null, ILogger logger = null)
        {
            logger?.LogTrace("Adding Botwin to application");

            ApplyGlobalBeforeHook(builder, options, logger);

            ApplyGlobalAfterHook(builder, options, logger);

            var routeBuilder = new RouteBuilder(builder);

            //Create a "startup scope" to resolve modules from
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                //Get all instances of BotwinModule to fetch and register declared routes
                foreach (var module in scope.ServiceProvider.GetServices<BotwinModule>())
                {
                    var moduleType = module.GetType();

                    var distinctPaths = module.Routes.Keys.Select(route =>
                    {
                        logger?.LogTrace($"Found and adding {moduleType.Name} with {route.verb} /{route.path} to routing");
                        return route.path;
                    }).Distinct();

                    foreach (var path in distinctPaths)
                    {
                        routeBuilder.MapRoute(path, CreateRouteHandler(path, moduleType, logger));
                    }
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static RequestDelegate CreateRouteHandler(string path, Type moduleType, ILogger logger)
        {
            return async ctx =>
            {
                var module = ctx.RequestServices.GetRequiredService(moduleType) as BotwinModule;

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
                    logger?.LogTrace("Executing module before hook");
                    shouldContinue = await module.Before(ctx);
                }

                if (shouldContinue)
                {
                    // run the route handler
                    logger?.LogTrace($"Executing module route handler for {ctx.Request.Method} {path}");
                    await routeHandler(ctx);

                    // run after handler
                    if (module.After != null)
                    {
                        logger?.LogTrace("Executing module after hook");
                        await module.After(ctx);
                    }
                }

                // run status code handler
                var statusCodeHandlers = ctx.RequestServices.GetServices<IStatusCodeHandler>();
                var scHandler = statusCodeHandlers.FirstOrDefault(x => x.CanHandle(ctx.Response.StatusCode));

                if (scHandler != null)
                {
                    logger?.LogTrace($"Executing {nameof(IStatusCodeHandler)} {scHandler.GetType().Name}");
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

        private static void ApplyGlobalAfterHook(IApplicationBuilder builder, BotwinOptions options, ILogger logger)
        {
            if (options?.After != null)
            {
                logger?.LogTrace("Applying global after hook");
                builder.Use(async (ctx, next) =>
                {
                    await next();
                    logger?.LogTrace("Executing global after hook");
                    await options.After(ctx);
                });
            }
            else
            {
                logger?.LogTrace("No global after hook found");
            }
        }

        private static void ApplyGlobalBeforeHook(IApplicationBuilder builder, BotwinOptions options, ILogger logger)
        {
            if (options?.Before != null)
            {
                logger?.LogTrace("Applying global before hook");
                builder.Use(async (ctx, next) =>
                {
                    logger?.LogTrace("Executing global before hook");
                    var carryOn = await options.Before(ctx); //TODO Check if return Task.CompletedTask will it continue
                    if (carryOn)
                    {
                        logger?.LogTrace("Executing next handler after global before hook");
                        await next();
                    }
                });
            }
            else
            {
                logger?.LogTrace("No global before hook found");
            }
        }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Botwin to.</param>
        /// <param name="assemblies">Optional array of <see cref="Assembly"/> to add to the services collection. If assemblies are not provided, Assembly.GetEntryAssembly is called.</param>
        /// <param name="loggerFactory">Optional <see cref="ILoggerFactory"/> to be passed for tracing of Botwin setup</param>
        public static void AddBotwin(this IServiceCollection services, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(BotwinExtensions));
            AddBotwin(services, logger);
        }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Botwin to.</param>
        /// <param name="assemblies">Optional array of <see cref="Assembly"/> to add to the services collection. If assemblies are not provided, Assembly.GetEntryAssembly is called.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> to be passed for tracing of Botwin setup</param>
        public static void AddBotwin(this IServiceCollection services, ILogger logger = null)
        {
            var assemblyCatalog = new DependencyContextAssemblyCatalog();

            var assemblies = assemblyCatalog.GetAssemblies();

            var validators = assemblies.SelectMany(ass => ass.GetTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);

            foreach (var validator in validators)
            {
                logger?.LogTrace($"Found {validator.FullName}");
                services.AddSingleton(typeof(IValidator), validator);
            }

            services.AddSingleton<IValidatorLocator, DefaultValidatorLocator>();

            services.AddRouting();

            var modules = assemblies.SelectMany(x => x.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    typeof(BotwinModule).IsAssignableFrom(t) &&
                    t != typeof(BotwinModule) &&
                    t.IsPublic
                ));

            foreach (var module in modules)
            {
                logger?.LogTrace($"Found {module.FullName}");
                services.AddScoped(module);
                services.AddScoped(typeof(BotwinModule), module);
            }

            var schs = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler)));
            foreach (var sch in schs)
            {
                logger?.LogTrace($"Found {sch.FullName}");
                services.AddScoped(typeof(IStatusCodeHandler), sch);
            }

            var responseNegotiators = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IResponseNegotiator).IsAssignableFrom(t) && t != typeof(IResponseNegotiator)));
            foreach (var negotiatator in responseNegotiators)
            {
                logger?.LogTrace($"Found {negotiatator.FullName}");
                services.AddSingleton(typeof(IResponseNegotiator), negotiatator);
            }

            services.AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>();
        }
    }
}
