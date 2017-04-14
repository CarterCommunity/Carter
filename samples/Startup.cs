namespace Botwin.Samples
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActorProvider, ActorProvider>();
            services.AddBotwin();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/errorhandler");

            app.UseBotwin(GetOptions());
        }

        private BotwinOptions GetOptions()
        {
            return new BotwinOptions(ctx => GetBeforeHook(ctx), ctx => GetAfterHook(ctx));
        }

        private Task<bool> GetBeforeHook(HttpContext ctx)
        {
            ctx.Request.Headers.Add("HOWDY", "FOLKS");
            return Task.FromResult(true);
        }

        private Task GetAfterHook(HttpContext ctx)
        {
            Console.WriteLine("We hit a route!");
            return Task.CompletedTask;
        }
    }
}