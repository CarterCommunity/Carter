namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotwinExtensions
    {
        /// <summary>
        /// Adds Botwin to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder)
        {
            return UseBotwin(builder, null);
        }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="options">A <see cref="BotwinOptions"/> instance.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder, BotwinOptions options)
        {
            ApplyGlobalBeforeHook(builder, options);

            ApplyGlobalAfterHook(builder, options);

            var routeBuilder = new RouteBuilder(builder);
            var systemRoutes = new List<(string verb, string route)>();

            //Create a "startup scope" to resolve modules from
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var modules = scope.ServiceProvider.GetServices<BotwinModule>();

                //Get all instances of BotwinModule to fetch and register declared routes
                foreach (var module in modules)
                {
                    var moduleType = module.GetType();

                    foreach (var route in module.Routes.Keys)
                    {
                        routeBuilder.MapVerb(route.verb, route.path, CreateRouteHandler(route, moduleType));

                        var strippedPath = route.path.EndsWith("/") ? route.path.Substring(0, route.path.Length - 1) : route.path;
                        systemRoutes.Add((route.verb, "/" + strippedPath));
                    }
                }

                builder.UseRouter(routeBuilder.Build());

                return builder.Use((ctx, next) => GetMethodNotAllowedHandler(ctx, next, systemRoutes));
            }
        }

        /// <summary>
        /// This method is only called if it's a valid 404 or 405
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="systemRoutes"></param>
        /// <returns></returns>
        private static async Task GetMethodNotAllowedHandler(HttpContext context, Func<Task> next, IEnumerable<(string verb, string route)> systemRoutes)
        {
            //Call the final pipeline which gets ASP.Net Core status code, usually a 404 in this case
            await next();

            var strippedPath = context.Request.Path.Value.EndsWith("/") && context.Request.Path.Value.Length > 1
                ? context.Request.Path.Value.Substring(0, context.Request.Path.Value.Length - 1)
                : context.Request.Path.Value;

            //ASP.Net Core will set a 405 response to 404. Let's check if it's a valid 404 first otherwise if we know about the route it's most likely a 405
            if (context.Response.StatusCode == 404 && systemRoutes.Any(x => x.route == strippedPath))
            {
                var verbsForPath = systemRoutes.Where(x => x.route == strippedPath).Select(y => y.verb);
                if (verbsForPath.All(x => x != context.Request.Method))
                {
                    context.Response.StatusCode = 405;
                }
            }
        }

        private static RequestDelegate CreateRouteHandler((string verb, string path) route, Type moduleType)
        {
            return async ctx =>
            {
                var module = ctx.RequestServices.GetRequiredService(moduleType) as BotwinModule;

                if (!module.Routes.TryGetValue((route.verb, route.path), out var routeHandler))
                {
                    throw new InvalidOperationException($"Route {route.verb} '{route.path}' was no longer found");
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
                    await routeHandler(ctx);

                    // run after handler
                    if (module.After != null)
                    {
                        await module.After(ctx);
                    }
                }

                // run status code handler
                var statusCodeHandlers = ctx.RequestServices.GetServices<IStatusCodeHandler>();
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

        private static void ApplyGlobalAfterHook(IApplicationBuilder builder, BotwinOptions options)
        {
            if (options?.After != null)
            {
                builder.Use(async (ctx, next) =>
                {
                    await next();
                    await options.After(ctx);
                });
            }
        }

        private static void ApplyGlobalBeforeHook(IApplicationBuilder builder, BotwinOptions options)
        {
            if (options?.Before != null)
            {
                builder.Use(async (ctx, next) =>
                {
                    var carryOn = await options.Before(ctx); //TODO Check if return Task.CompletedTask will it continue
                    if (carryOn)
                    {
                        await next();
                    }
                });
            }
        }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Botwin to.</param>
        /// <param name="assemblies">Optional array of <see cref="Assembly"/> to add to the services collection. If assemblies are not provided, Assembly.GetEntryAssembly is called.</param>
        public static void AddBotwin(this IServiceCollection services, params Assembly[] assemblies)
        {
            assemblies = assemblies.Any() ? assemblies : new[] { Assembly.GetEntryAssembly() };

            var validators = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);

            foreach (var validator in validators)
            {
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
                services.AddScoped(module);
                services.AddScoped(typeof(BotwinModule), module);
            }

            var schs = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler)));
            foreach (var sch in schs)
            {
                services.AddScoped(typeof(IStatusCodeHandler), sch);
            }

            var responseNegotiators = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IResponseNegotiator).IsAssignableFrom(t) && t != typeof(IResponseNegotiator)));
            foreach (var negotiatator in responseNegotiators)
            {
                services.AddSingleton(typeof(IResponseNegotiator), negotiatator);
            }

            services.AddSingleton(typeof(IResponseNegotiator), new DefaultJsonResponseNegotiator());
        }
    }
}
