namespace Botwin.Samples
{
    using Microsoft.AspNetCore.Builder;
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
            app.UseBotwin();
        }
    }
}