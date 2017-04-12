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
            // RequestDelegate a = async (ctx) =>
            // {
            //     if (1 == 1)
            //     {
            //         ctx.Response.StatusCode = 500;
            //         await ctx.Response.WriteAsync("I'm done");
            //         ctx.Abort();
            //         return;
            //     }
            // };
            // RequestDelegate b = async (ctx) => await ctx.Response.WriteAsync("Please don't hit this"); ;
            // RequestDelegate c = a += b;
            // app.Run(c);
            app.UseBotwin();
        }
    }
}