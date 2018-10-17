namespace Carter.Tests
{
    using Microsoft.AspNetCore.Http;

    public class ShortCircuitModule : CarterModule
    {
        public ShortCircuitModule()
        {
            this.Before += async ctx =>
            {
                await ctx.Response.WriteAsync("NoAccessBefore");
                return false;
            };
            this.Get("/noaccess", async ctx => { await ctx.Response.WriteAsync("Not Accessible"); });
        }
    }
}
