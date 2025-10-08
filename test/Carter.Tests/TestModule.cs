namespace Carter.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Carter.ModelBinding;
    using Carter.Request;
    using Carter.Response;
    using Carter.Tests.ModelBinding;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;

    public class TestModule : ICarterModule
    {
        private Guid instanceId;
        
        public TestModule()
        {
            this.instanceId = Guid.NewGuid();
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });

            app.MapGet("/querystring", async (HttpContext ctx) =>
            {
                var id = ctx.Request.Query.As<int>("id");
                await ctx.Response.WriteAsync($"Managed to parse an int {id}");
            });

            app.MapGet("/nullablequerystring", async (HttpContext ctx) =>
            {
                var id = ctx.Request.Query.As<int?>("id");
                await ctx.Response.WriteAsync($"Managed to parse a Nullable<int> {id}");
            });

            app.MapGet("/multiquerystring", async (HttpContext ctx) =>
            {
                var id = ctx.Request.Query.AsMultiple<int>("id");
                await ctx.Response.WriteAsync($"Managed to parse multiple ints {id.Count()}");
            });
            
            app.MapGet("/multimulti", async (HttpContext ctx) =>
            {
                var id = ctx.Request.Query.AsMultiple<int[]>("id");
                await ctx.Response.WriteAsync($"Managed to parse multiple ints {id.Count()}");
            });

            app.MapGet("/nullablemultiquerystring", async (HttpContext ctx) =>
            {
                var id = ctx.Request.Query.AsMultiple<int?>("id");
                await ctx.Response.WriteAsync($"Managed to parse multiple Nullable<int>s {id.Count()}");
            });

            app.MapGet("/querystringdefault", async (HttpContext ctx) =>
            {
                var id = ctx.Request.Query.As("id", 69);
                await ctx.Response.WriteAsync($"Managed to parse default int {id}");
            });

            app.MapPost("/asstringasync", async (HttpContext ctx) =>
            {
                var content = await ctx.Request.Body.AsStringAsync();
                await ctx.Response.WriteAsync(content);
            });
            
            app.MapPost("/bindandsave", async (HttpRequest req, HttpResponse res) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath);

                await res.Negotiate(new ExtensionTests.PathTestModel { Path = filePath });
            });

            app.MapPost("/bindandsavecustomname", async (HttpRequest req, HttpResponse res) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath, "mycustom.txt");

                await res.Negotiate(new ExtensionTests.PathTestModel { Path = filePath });
            });

            app.MapGet("405test", context => context.Response.WriteAsync("hi"));
            app.MapGet("405testwithslash/", context => context.Response.WriteAsync("hi"));
            app.MapGet("405multiple", context => context.Response.WriteAsync("405multiple-get"));
            app.MapPost("405multiple", context => context.Response.WriteAsync("405multiple-post"));

            app.MapGet("/parameterized/{name:alpha}", ctx => ctx.Response.WriteAsync("echo " + ctx.GetRouteData().Values["name"]));
            app.MapGet("/parameterized/{id:int}", (HttpContext ctx, int id) => ctx.Response.WriteAsync("echo " + id));
            app.MapGet("/parameterized/{id:guid}", (HttpContext ctx, Guid id) => ctx.Response.WriteAsync("echo " + id));
            app.MapGet("/parameterized/{id:datetime}", (HttpContext ctx, DateTime id) => ctx.Response.WriteAsync("echo " + id.ToString("dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture)));

            app.MapPost("/", async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });
            app.MapPut("/", async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });
            app.MapDelete("/", async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });
            app.MapMethods("/head", new[]{"HEAD"}, async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });
            app.MapMethods("/", new[]{"PATCH"}, async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });
            app.MapMethods("/", new[]{"OPTIONS"}, async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello"); });

            app.MapPost<TestModel>("/endpointfilter", ([FromBody]TestModel testModel,IDependency dependency) => "POST");
            app.MapPut<TestModel>("/endpointfilter", (IDependency dependency, [FromBody]TestModel testModel) => "PUT");
        }
    }
    public static class NestedTestModule
    {
        public class TestModule : ICarterModule
        {
            private Guid instanceId;

            public TestModule()
            {
                this.instanceId = Guid.NewGuid();
            }

            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapGet("/nested", async (HttpContext ctx) => { await ctx.Response.WriteAsync("Hello Nested"); });
            }
        }
    }

}
