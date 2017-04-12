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
            var routeBuilder = new RouteBuilder(builder);

            //Invoke so ctors are called that adds routes to IRouter
            var srvs = builder.ApplicationServices.GetServices<BotwinModule>();

            foreach (var module in srvs)
            {
                foreach (var route in module.Routes)
                {
                    Func<HttpRequest, HttpResponse, RouteData, Task> handler;
                    if (module.Before != null)
                    {
                        Func<HttpRequest, HttpResponse, RouteData, Task> newhandler = async (req, res, routeData) =>
                        {
                            var beforeResult = await module.Before(req, res, routeData);
                            if (beforeResult == null)
                            {
                                return;
                            }
                            await route.Item3(req, res, routeData);
                        };
                        handler = newhandler;
                    }
                    else
                    {
                        handler = route.Item3;
                    }

                    if (module.After != null)
                    {
                        handler = handler += module.After;
                    }

                    routeBuilder.MapVerb(route.Item1, route.Item2, handler);
                }
            }
            return builder.UseRouter(routeBuilder.Build());
        }

        public static void AddBotwin(this IServiceCollection services)
        {
            services.AddRouting();

            var modules = Assembly.GetEntryAssembly().GetTypes().Where(t => typeof(BotwinModule).IsAssignableFrom(t) && t != typeof(BotwinModule));
            foreach (var module in modules)
            {
                services.AddTransient(typeof(BotwinModule), module);
            }
        }

        public static int AsInt(this RouteValueDictionary rvd, string key)
        {
            return Convert.ToInt32(rvd[key]);
        }

        public static async Task Negotiate(this HttpResponse response, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (response.HttpContext.Request.GetTypedHeaders().Accept.Any(x => x.MediaType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                await response.WriteAsync(JsonConvert.SerializeObject(obj));
            }
            else if (response.HttpContext.Request.GetTypedHeaders().Accept.Any(x => x.MediaType.IndexOf("xml", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                //I don't know I didn't go to Burger King
            }
            await response.WriteAsync(JsonConvert.SerializeObject(obj));
        }

        public static (ValidationResult ValidationResult, T Data) BindAndValidate<T>(this HttpRequest request)
        {
            var data = request.Bind<T>();
            if (data == null)
            {
                data = Activator.CreateInstance<T>();
            }
            var validatorType = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(t => t.GetTypeInfo().BaseType != null &&
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