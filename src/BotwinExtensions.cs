namespace Botwin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class BotwinExtensions
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder)
        {
            return UseBotwin(null);
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

                    handler = module.Before != null ? CreateModuleBeforeHandler(module, route) : route.Item3;

                    if (module.After != null)
                    {
                        handler += module.After;
                    }

                    var finalHandler = CreateFinalHandler(handler, statusCodeHandlers);

                    routeBuilder.MapVerb(route.Item1, route.Item2, finalHandler);
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static Func<HttpRequest, HttpResponse, RouteData, Task> CreateFinalHandler(Func<HttpRequest, HttpResponse, RouteData, Task> handler, IEnumerable<IStatusCodeHandler> statusCodeHandlers)
        {
            Func<HttpRequest, HttpResponse, RouteData, Task> finalHandler = async (req, res, routeData) =>
            {
                if (req.Method == "HEAD")
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

                if (req.Method == "HEAD")
                {
                    var length = res.Body.Length;
                    res.Body.SetLength(0);
                    res.ContentLength = length;
                }
            };
            return finalHandler;
        }

        private static Func<HttpRequest, HttpResponse, RouteData, Task> CreateModuleBeforeHandler(BotwinModule module, Tuple<string, string, Func<HttpRequest, HttpResponse, RouteData, Task>> route)
        {
            Func<HttpRequest, HttpResponse, RouteData, Task> beforeHandler = async (req, res, routeData) =>
            {
                var beforeResult = await module.Before(req, res, routeData);
                if (beforeResult == null)
                {
                    return;
                }
                await route.Item3(req, res, routeData);
            };

            return beforeHandler;
        }

        private static void ApplyGlobalAfterHook(IApplicationBuilder builder, BotwinOptions options)
        {
            if (options != null && options.After != null)
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
            if (options != null && options.Before != null)
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

        public static void AddBotwin(this IServiceCollection services)
        {
            services.AddRouting();

            var modules = Assembly.GetEntryAssembly().GetTypes().Where(t => typeof(BotwinModule).IsAssignableFrom(t) && t != typeof(BotwinModule));
            foreach (var module in modules)
            {
                services.AddTransient(typeof(BotwinModule), module);
            }

            var schs = Assembly.GetEntryAssembly().GetTypes().Where(t => typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler));
            foreach (var sch in schs)
            {
                services.AddTransient(typeof(IStatusCodeHandler), sch);
            }
        }

        public static int AsInt(this RouteData routeData, string key)
        {
            return Convert.ToInt32(routeData.Values[key]);
        }

        public static async Task Negotiate(this HttpResponse response, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (response.HttpContext.Request.GetTypedHeaders().Accept.Any(x => x.MediaType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                response.ContentType = "application/json; charset=utf-8";
                await response.WriteAsync(JsonConvert.SerializeObject(obj));
                return;
            }

            if (response.HttpContext.Request.GetTypedHeaders().Accept.Any(x => x.MediaType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                //I don't know I didn't go to Burger King
            }

            //Default to JSON
            response.ContentType = "application/json; charset=utf-8";
            await response.WriteAsync(JsonConvert.SerializeObject(obj));
        }

        public static (ValidationResult ValidationResult, T Data) BindAndValidate<T>(this HttpRequest request)
        {
            var data = request.Bind<T>();
            if (data == null)
            {
                data = Activator.CreateInstance<T>();
            }
            var validatorType = Assembly.GetEntryAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.GetTypeInfo().BaseType != null &&
                    t.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                    t.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>) &&
                    t.Name.Equals(typeof(T).Name + "Validator", StringComparison.OrdinalIgnoreCase));

            IValidator validator = (IValidator)Activator.CreateInstance(validatorType);
            var result = validator.Validate(data);
            return (result, data);
        }

        public static T Bind<T>(this HttpRequest request)
        {
            using (var streamReader = new StreamReader(request.Body))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return JsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }

        public static IEnumerable<dynamic> GetFormattedErrors(this ValidationResult result)
        {
            return result.Errors.Select(x => new { x.PropertyName, x.ErrorMessage });
        }
    }
}