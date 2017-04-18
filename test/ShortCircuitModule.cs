namespace Botwin.Tests
{
    using Microsoft.AspNetCore.Http;
    public class ShortCircuitModule : BotwinModule
    {
        public ShortCircuitModule()
        {
            this.Before = async (req, res, routeData) => { await res.WriteAsync("NoAccessBefore"); return null; };
            this.Get("/noaccess", async (req, res, routeData) => { await res.WriteAsync("Not Accessible"); });
        }
    }
}
