namespace Botwin
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public static class BotwinExtensions
    {
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder)
        {
            var routeBuilder = new RouteBuilder(builder);

            //Invoke so ctors are called that adds routes to IRouter
            var srvs = builder.ApplicationServices.GetServices<BotwinModule>();

            foreach (var module in srvs)
            {
                foreach (var route in module.Routes)
                {
                    routeBuilder.MapVerb(route.Item1, route.Item2, route.Item3);
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
        }
    }
}