namespace Botwin.Samples
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;

    public class HomeModule : BotwinModule
    {
        public HomeModule()
        {
            this.Get("/", async (req, res, routeData) =>
            {
                await res.WriteAsync("There's no place like 127.0.0.1");
            });

            this.Get("/error", (req, res, routeData) =>
            {
                throw new Exception("oops");
            });

            this.Get("/errorhandler", async (req, res, routeData) =>
            {
                string error = string.Empty;
                var feature = req.HttpContext.Features.Get<IExceptionHandlerFeature>();
                if (feature != null)
                {
                    error = feature.Error.ToString();
                }
                await res.WriteAsync($"There has been an error{Environment.NewLine}{error}");
            });

            this.After = (req, res, routeData) =>
            {
                Console.WriteLine("Catch you later!");
                return Task.CompletedTask;
            };
        }
    }
}