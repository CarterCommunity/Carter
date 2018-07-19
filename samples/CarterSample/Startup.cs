namespace CarterSample
{
    using System;
    using System.Threading.Tasks;
    using Carter;
    using CarterSample.Features.Actors;
    using CarterSample.Features.CastMembers;
    using CarterSample.Features.Errors;
    using CarterSample.Features.FunctionalProgramming;
    using CarterSample.Features.Home;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter(bootstrapper => 
                bootstrapper
                    .RegisterModules(
                        new FunctionalProgrammingModule(),
                        new ActorsModule(new ActorProvider()),
                        new CastMemberModule(),
                        new HomeModule(),
                        new ErrorModule())
                    .RegisterValidators(new ActorValidator(), new DirectorValidator())
                    .RegisterResponseNegotiators(new DefaultJsonResponseNegotiator())
                    .RegisterStatusCodeHandlers(new ConflictStatusCodeHandler())
            );
            
            services.AddSingleton<IActorProvider, ActorProvider>(); // only required to test carted scoped
        }

        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            var appconfig = new AppConfiguration();
            config.Bind(appconfig);

            app.UseExceptionHandler("/errorhandler");

            app.UseCarter(this.GetOptions());
        }

        private CarterOptions GetOptions()
        {
            return new CarterOptions(ctx => this.GetBeforeHook(ctx), ctx => this.GetAfterHook(ctx));
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
