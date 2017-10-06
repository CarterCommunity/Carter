namespace Botwin.Tests
{
    using System.Linq;
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
            this.Get("/querystring", async ctx =>
            {
                var id = ctx.Request.Query.As<int>("id");
                await ctx.Response.WriteAsync($"Managed to parse an int {id}");
            });
            this.Get("/multiquerystring", async ctx =>
            {
                var id = ctx.Request.Query.AsMultiple<int>("id");
                
                await ctx.Response.WriteAsync($"Managed to parse multiple ints {id.Count()}");
            });
            this.Post("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Put("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Delete("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Head("/head", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Patch("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Options("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
        }
    }
}
