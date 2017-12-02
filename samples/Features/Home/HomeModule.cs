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
                res.StatusCode = 409;
                await res.WriteAsync("There's no place like 127.0.0.1");
            });

            this.After = (ctx) =>
            {
                Console.WriteLine("Catch you later!");
                return Task.CompletedTask;
            };
        }
    }
}