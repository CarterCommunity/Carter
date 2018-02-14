namespace BotwinAndMVC
{
    using Botwin;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBotwin();
            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseBotwin();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
