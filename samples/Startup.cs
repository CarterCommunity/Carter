namespace Botwin.Samples
{
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
            //app.UseStatusCodePages((subapp) => subapp.Run((ctx) => ctx.Response.WriteAsync("409")));

            app.UseBotwin();
        }
    }
}