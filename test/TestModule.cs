namespace Botwin.Tests
{
    using System.Linq;
    using System.Text;
    using Botwin.Request;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

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
                var foo = ctx.Request.Body.AsStringAsync();
                var bar = ctx.Request.Body.AsStringAsync(Encoding.UTF8);

                await Task.WhenAll(foo, bar);

                await ctx.Response.WriteAsync($"This is {nameof(foo)}: {await foo}, and {nameof(bar)} doesn't matter now.");
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
