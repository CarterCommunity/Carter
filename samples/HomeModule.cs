namespace Botwin.Samples
{
    using System;
    using System.Threading.Tasks;
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

            this.After = (req, res, routeData) =>
            {
                Console.WriteLine("Catch you later!");
                return Task.CompletedTask;
            };
        }
    }
}