namespace BotwinTemplate
{
    using Botwin;
    using Microsoft.AspNetCore.Http;

    public class HomeModule : BotwinModule
    {
        public HomeModule()
        {
            Get("/", async(req,res,routeData) => await res.WriteAsync("Hello from Botwin!"));
        }
    }
}
