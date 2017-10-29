namespace Botwin.Tests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class ShortCircuitModule : BotwinModule
    {
        public ShortCircuitModule()
        {
            this.Before = async (ctx) =>
            {
                await ctx.Response.WriteAsync("NoAccessBefore");
                return false;
            };
            this.Get("/noaccess", async (ctx) =>
            {
                
                await ctx.Response.WriteAsync("Not Accessible");
            });
        }
    }
}
