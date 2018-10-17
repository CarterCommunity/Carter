namespace Carter.Tests
{
    using Microsoft.AspNetCore.Http;

    public class MultipleShortCircuitModule : CarterModule
    {
        public MultipleShortCircuitModule()
        {
            this.Before += async ctx =>
            {
                await ctx.Response.WriteAsync("FirstBefore");
                return true;
            };
            
            this.Before += async ctx =>
            {
                await ctx.Response.WriteAsync("SecondBefore");
                return true;
            };
            
            this.Get("/multipleshortcircuits", async ctx => { await ctx.Response.WriteAsync("Multiple"); });
        }
    }
}