namespace Carter.Tests
{
    using Microsoft.AspNetCore.Http;

    public class MultipleShortCircuitOnOff : CarterModule
    {
        public MultipleShortCircuitOnOff()
        {
            this.Before += async ctx =>
            {
                await ctx.Response.WriteAsync("OffBefore");
                return false;
            };
            
            this.Before += async ctx =>
            {
                await ctx.Response.WriteAsync("ShouldNeverSeeThis");
                return true;
            };
            
            this.Get("/multipleonoff", async ctx => { await ctx.Response.WriteAsync("Not Accessible"); });
        }
    }
}