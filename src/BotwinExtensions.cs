namespace Botwin
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

    public static class BotwinExtensions
    {
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder)
        {
            return UseBotwin(builder, null);
        }

        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder, BotwinOptions options)
        {
            //Invoke so ctors are called that adds routes to IRouter
            var srvs = builder.ApplicationServices.GetServices<BotwinModule>();

            Add405Handler(builder, srvs);

            ApplyGlobalBeforeHook(builder, options);

            ApplyGlobalAfterHook(builder, options);

            var routeBuilder = new RouteBuilder(builder);

            //Cache status code handlers
            var statusCodeHandlers = builder.ApplicationServices.GetServices<IStatusCodeHandler>();

            foreach (var module in srvs)
            {
                foreach (var route in module.Routes)
                {
                    RequestDelegate handler;

                    handler = CreateModuleBeforeAfterHandler(module, route);

                    var finalHandler = CreateFinalHandler(handler, statusCodeHandlers);

                    routeBuilder.MapVerb(route.verb, route.path, finalHandler);
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static void Add405Handler(IApplicationBuilder builder, IEnumerable<BotwinModule> srvs)
        {
            var systemRoutes = new List<(string verb, string route)>();

            foreach (var module in srvs)
            {
                foreach (var route in module.Routes)
                {
                    systemRoutes.Add((route.verb, "/" + route.path));
                }
            }

            builder.Use(async (context, next) =>
            {
                var strippedPath = context.Request.Path.Value.Substring(0, context.Request.Path.Value.Length - 1);
                var verbsForPath = systemRoutes.Where(x => x.route.StartsWith(strippedPath)).Select(y => y.verb);
                if (verbsForPath.All(x => x != context.Request.Method))
                {
                    context.Response.StatusCode = 405;
                    return;
                }
                await next();
            });
        }

        private static RequestDelegate CreateFinalHandler(RequestDelegate handler, IEnumerable<IStatusCodeHandler> statusCodeHandlers)
        {
            RequestDelegate finalHandler = async (ctx) =>
            {
                if (HttpMethods.IsHead(ctx.Request.Method))
                {
                    //Cannot read the default stream once WriteAsync has been called on it
                    ctx.Response.Body = new MemoryStream();
                }

                await handler(ctx);

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
            return finalHandler;
        }

        private static RequestDelegate CreateModuleBeforeAfterHandler(BotwinModule module, (string verb, string path, RequestDelegate handler) route)
        {
            RequestDelegate afterHandler = async (context) =>
            {
                if (module.Before != null)
                {
                    var shouldContinue = await module.Before(context);
                    if (!shouldContinue)
                    {
                        return;
                    }
                }

                await route.handler(context);

                if (module.After != null)
                {
                    await module.After(context);
                }
            };

            return afterHandler;
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

            var modules = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(BotwinModule).IsAssignableFrom(t) && t != typeof(BotwinModule)));
            foreach (var module in modules)
            {
                services.AddTransient(typeof(BotwinModule), module);
            }

            var schs = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler)));
            foreach (var sch in schs)
            {
                services.AddTransient(typeof(IStatusCodeHandler), sch);
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
