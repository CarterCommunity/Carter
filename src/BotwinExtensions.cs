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
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class BotwinExtensions
    {
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder)
        {
            return UseBotwin(builder, null);
        }

        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder, BotwinOptions options)
        {
            ApplyGlobalBeforeHook(builder, options);

            ApplyGlobalAfterHook(builder, options);

            var routeBuilder = new RouteBuilder(builder);

            //Invoke so ctors are called that adds routes to IRouter
            var srvs = builder.ApplicationServices.GetServices<BotwinModule>();

            //Cache status code handlers
            var statusCodeHandlers = builder.ApplicationServices.GetServices<IStatusCodeHandler>();

            foreach (var module in srvs)
            {
                foreach (var route in module.Routes)
                {
                    Func<HttpRequest, HttpResponse, RouteData, Task> handler;

                    handler = module.Before != null ? CreateModuleBeforeHandler(module, route) : route.handler;

                    if (module.After != null)
                    {
                        handler += module.After;
                    }

                    var finalHandler = CreateFinalHandler(handler, statusCodeHandlers);

                    routeBuilder.MapVerb(route.verb, route.path, finalHandler);
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static Func<HttpRequest, HttpResponse, RouteData, Task> CreateFinalHandler(Func<HttpRequest, HttpResponse, RouteData, Task> handler, IEnumerable<IStatusCodeHandler> statusCodeHandlers)
        {
            Func<HttpRequest, HttpResponse, RouteData, Task> finalHandler = async (req, res, routeData) =>
            {
                if (HttpMethods.IsHead(req.Method))
                {
                    //Cannot read the default stream once WriteAsync has been called on it
                    res.Body = new MemoryStream();
                }

                await handler(req, res, routeData);

                var scHandler = statusCodeHandlers.FirstOrDefault(x => x.CanHandle(res.StatusCode));

                if (scHandler != null)
                {
                    await scHandler.Handle(req.HttpContext);
                }

                if (HttpMethods.IsHead(req.Method))
                {
                    var length = res.Body.Length;
                    res.Body.SetLength(0);
                    res.ContentLength = length;
                }
            };
            return finalHandler;
        }

        private static Func<HttpRequest, HttpResponse, RouteData, Task> CreateModuleBeforeHandler(BotwinModule module, (string verb, string path, Func<HttpRequest, HttpResponse, RouteData, Task> handler) route)
        {
            Func<HttpRequest, HttpResponse, RouteData, Task> beforeHandler = async (req, res, routeData) =>
            {
                var beforeResult = await module.Before(req, res, routeData);
                if (beforeResult == null)
                {
                    return;
                }
                await route.handler(req, res, routeData);
            };

            return beforeHandler;
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
                    var carryOn = await options.Before(ctx);
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
            
            var validators = assemblies.SelectMany(x=>
                x.GetTypes())
                .Where(t => t.GetTypeInfo().BaseType != null &&
                    t.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                    t.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>));

            services.AddSingleton(typeof(IEnumerable<Type>), validators);


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
