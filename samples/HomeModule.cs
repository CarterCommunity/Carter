namespace Botwin.Samples
{
    using Microsoft.AspNetCore.Http;

    public class HomeModule : BotwinModule
    {
        public HomeModule()
        {
            this.Get("/", async (req, res, routeData) => await res.WriteAsync("hi"));
        }
    }
}