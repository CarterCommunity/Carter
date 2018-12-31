namespace CarterSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Carter;
    using CarterSample.Features.Actors;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActorProvider, ActorProvider>();

            services.AddCarter();
        }

        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            var appconfig = new AppConfiguration();
            config.Bind(appconfig);

            app.UseExceptionHandler("/errorhandler");

            app.UseSwaggerUI(opt =>
            {
                opt.RoutePrefix = "openapi/ui";
                opt.SwaggerEndpoint("/openapi", "Carter OpenAPI Sample");
            });

            app.UseCarter(this.GetOptions(app.ServerFeatures.Get<IServerAddressesFeature>().Addresses));
        }

        private CarterOptions GetOptions(ICollection<string> addresses)
        {
            return new CarterOptions(ctx => this.GetBeforeHook(ctx), ctx => this.GetAfterHook(ctx), new OpenApiOptions(addresses));
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
