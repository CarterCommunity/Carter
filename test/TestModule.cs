namespace Botwin.Tests
{
    using System.Linq;
    using Botwin.Request;
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

            this.Get("/nullablequerystring", async ctx =>
            {
                var id = ctx.Request.Query.As<int?>("id");
                await ctx.Response.WriteAsync($"Managed to parse a Nullable<int> {id}");
            });

            this.Get("/multiquerystring", async ctx =>
            {
                var id = ctx.Request.Query.AsMultiple<int>("id");
                await ctx.Response.WriteAsync($"Managed to parse multiple ints {id.Count()}");
            });

            this.Get("/nullablemultiquerystring", async ctx =>
            {
                var id = ctx.Request.Query.AsMultiple<int?>("id");
                await ctx.Response.WriteAsync($"Managed to parse multiple Nullable<int>s {id.Count()}");
            });

            this.Get("/querystringdefault", async ctx =>
            {
                var id = ctx.Request.Query.As<int>("id", 69);
                await ctx.Response.WriteAsync($"Managed to parse default int {id}");
            });

            this.Post("/asstring", async ctx =>
            {
                var content = ctx.Request.Body.AsString();
                await ctx.Response.WriteAsync(content);
            });

            this.Post("/asstringasync", async ctx =>
            {
                var content = await ctx.Request.Body.AsStringAsync();
                await ctx.Response.WriteAsync(content);
            });
            
            this.Get("405test", context => context.Response.WriteAsync("hi"));
            this.Get("405testwithslash/", context => context.Response.WriteAsync("hi"));

            
            this.Post("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Put("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Delete("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Head("/head", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Patch("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
            this.Options("/", async (ctx) => { await ctx.Response.WriteAsync("Hello"); });
        }
    }
}
