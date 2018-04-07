namespace Carter.Tests
{
    using Microsoft.AspNetCore.Http;

    public class TestModuleBaseClass : CarterModule
    {
        public TestModuleBaseClass() : base("/test")
        {
            this.Get("/", async ctx => { await ctx.Response.WriteAsync("Hello"); });
            this.Post("/", async ctx => { await ctx.Response.WriteAsync("Hello"); });
            this.Put("/", async ctx => { await ctx.Response.WriteAsync("Hello"); });
            this.Delete("/", async ctx => { await ctx.Response.WriteAsync("Hello"); });
            this.Head("/head", async ctx => { await ctx.Response.WriteAsync("Hello"); });
            this.Patch("/", async ctx => { await ctx.Response.WriteAsync("Hello"); });
            this.Options("/", async ctx => { await ctx.Response.WriteAsync("Hello"); });
        }
    }
}
