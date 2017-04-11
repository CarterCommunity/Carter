namespace Botwin.Samples
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class HomeModule : BotwinModule
    {
        public HomeModule()
        {
            this.Before = (req, res, routeData) =>
            {
                Console.WriteLine("inb4 yo!");
                return Task.CompletedTask;
            };

            this.Get("/", async (req, res, routeData) =>
            {
                await res.WriteAsync("There's no place like 127.0.0.1");
            });

            this.After = (req, res, routeData) =>
            {
                Console.WriteLine("Catch you later!");
                return Task.CompletedTask;
            };
        }
    }
}