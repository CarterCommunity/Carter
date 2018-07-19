namespace Carter
{
    using System;
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
        /// Adds Carter to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Carter to.</param>
        /// <param name="configure">Allows <see cref="CarterBootstrapper"/> to be configured.</param>
        public static void AddCarter(this IServiceCollection services, Action<CarterBootstrapper> configure)
        {
            // hack to test without changing all tests
            // AddCarterScoped(services);
            // return;
            
            var bootstrapper = new CarterBootstrapper();
            configure(bootstrapper);
            if (bootstrapper.ResponseNegotiators.All(x => x.GetType() != typeof(DefaultJsonResponseNegotiator)))
            {
                bootstrapper.RegisterResponseNegotiators(new DefaultJsonResponseNegotiator());    
            }            
            services.AddSingleton<IValidatorLocator>(new CarterValidatorLocator(bootstrapper.Validators));
            services.AddSingleton<IEnumerable<IResponseNegotiator>>(bootstrapper.ResponseNegotiators);
            services.AddSingleton<ICarterBootstrapper>(bootstrapper);
            services.AddRouting();
        }

        /// <summary>
        /// Adds Carter to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Carter to.</param>
        /// <param name="configure">Allows <see cref="CarterBootstrapper"/> to be configured.</param>
        public static void AddCarterScoped(this IServiceCollection services)
        {
            var scanner = new CarterDependencyScanner();

            foreach (var validator in scanner.ScanValidators())
            {
                services.AddSingleton(typeof(IValidator), validator);
            }

            services.AddSingleton<IValidatorLocator, CarterValidatorLocator>();

            services.AddRouting();

            foreach (var module in scanner.ScanModules())
            {
                services.AddScoped(module);
                services.AddScoped(typeof(CarterModule), module);
            }

            foreach (var statusCodeHandler in scanner.ScanStatusCodeHandlers())
            {
                services.AddScoped(typeof(IStatusCodeHandler), statusCodeHandler);
            }

            foreach (var negotiator in scanner.ScanResponseNegotiators())
            {
                services.AddSingleton(typeof(IResponseNegotiator), negotiator);
            }

            services.AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>();
        }
    
        /// <summary>
        /// Adds Carter to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="options">A <see cref="CarterOptions"/> instance.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseCarter(this IApplicationBuilder builder, CarterOptions options = null)
        {
            var loggerFactory = builder.ApplicationServices.GetService<ILoggerFactory>();      
            
            ApplyGlobalBeforeHook(builder, options, loggerFactory.CreateLogger("Carter.GlobalBeforeHook"));
            ApplyGlobalAfterHook(builder, options, loggerFactory.CreateLogger("Carter.GlobalAfterHook"));

            var routeBuilder = new RouteBuilder(builder);
            var bootstrapper = builder.ApplicationServices.GetService<ICarterBootstrapper>();
            if (bootstrapper != null)
            {
                CarterDiagnostics.LogDiscoveredCarterTypes(
                    bootstrapper, loggerFactory.CreateLogger("CarterDiagnostics"));

                foreach (var route in bootstrapper.Routes.Values)
                {
                    var moduleLogger = builder.ApplicationServices
                        .GetService<ILoggerFactory>()
                        .CreateLogger(route.Module);
                
                    routeBuilder.MapRoute(route.Path, 
                        HandleRequest(route.Path, bootstrapper.Routes, bootstrapper.StatusCodeHandlers, moduleLogger));
                }
            }
            else
            {
                CarterDiagnostics.LogDiscoveredCarterTypes(
                    builder.ApplicationServices, loggerFactory.CreateLogger("CarterDiagnostics"));

                // create a "startup scope" to resolve modules from
                using (var scope = builder.ApplicationServices.CreateScope())
                {
                    var statusCodeHandlers = scope.ServiceProvider.GetServices<IStatusCodeHandler>();

                    // get all instances of CarterModule to fetch and register declared routes
                    foreach (var module in scope.ServiceProvider.GetServices<CarterModule>())
                    {
                        var moduleLogger = scope.ServiceProvider
                            .GetService<ILoggerFactory>()
                            .CreateLogger(module.GetType());
                    
                        var distinctPaths = module.Routes.Keys.Select(route => route.path).Distinct();
                        foreach (var path in distinctPaths)
                        {
                            routeBuilder.MapRoute(path, 
                                HandleRequest(path, module.Routes, statusCodeHandlers, moduleLogger));
                        }
                    }
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static RequestDelegate HandleRequest(
            string path, IReadOnlyDictionary<(string verb, string path), CarterRoute> routes, 
            IEnumerable<IStatusCodeHandler> statusCodeHandlers,ILogger logger)
        {
            return async ctx =>
            {
                if (!routes.TryGetValue((ctx.Request.Method, path), out var carterRoute))
                {
                    // if the path was registered but a handler matching the
                    // current method was not found, return MethodNotFound
                    ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
                }

                if (HttpMethods.IsHead(ctx.Request.Method))
                {
                    // cannot read the default stream once WriteAsync has been called on it
                    ctx.Response.Body = new MemoryStream();
                }
                   
                logger.LogDebug("Executing module route handler for {Method} /{Path}", ctx.Request.Method, carterRoute.Path);
                await carterRoute.Handler(ctx);

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
    }
}
