namespace Botwin.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BotwinSample;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        private readonly ILoggerFactory loggerFactory;

        public Startup(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActorProvider, ActorProvider>();
            services.AddBotwin(this.loggerFactory);
        }

        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            var appconfig = new AppConfiguration();
            config.Bind(appconfig);

            app.UseExceptionHandler("/errorhandler");

            app.UseBotwin(this.GetOptions(), this.loggerFactory.CreateLogger(typeof(BotwinExtensions)));
        }

        private BotwinOptions GetOptions()
        {
            return new BotwinOptions(ctx => this.GetBeforeHook(ctx), ctx => this.GetAfterHook(ctx));
        }

        private Task<bool> GetBeforeHook(HttpContext ctx)
        {
            ctx.Request.Headers["HOWDY"] = "FOLKS";
            return Task.FromResult(true);
        }

        private Task GetAfterHook(HttpContext ctx)
        {
            Console.WriteLine("We hit a route!");
            return Task.CompletedTask;
        }
    }
}
