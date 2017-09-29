namespace Botwin.Tests
{
    using Microsoft.AspNetCore.Http;

    public class TestModule : BotwinModule
    {
        public TestModule()
        {
            this.Before = async (ctx) =>
            {
                await ctx.Response.WriteAsync("Before");
                return true;
            };
            this.After = async (ctx) => { await ctx.Response.WriteAsync("After"); };
            this.Get("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Post("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Put("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Delete("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Head("/head", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Patch("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Options("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
        }
    }
}
