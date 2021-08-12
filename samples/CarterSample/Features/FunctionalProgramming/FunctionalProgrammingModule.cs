namespace CarterSample.Features.FunctionalProgramming
{
    using System;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Request;
    using Carter.Response;
    using Microsoft.AspNetCore.Routing;

    public class FunctionalProgrammingModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var basePath = "/functional";

            app.MapGet($"{basePath}/directors", (HttpResponse res) =>
            {
                var handler = RouteHandlers.ListDirectorsHandler;

                var directors = handler();

                if (directors == null)
                {
                    res.StatusCode = 403;
                    return Task.CompletedTask;
                }

                return res.AsJson(directors);
            });

            app.MapGet($"{basePath}/directors/{{id:int}}", (int id) =>
            {
                var handler = RouteHandlers.GetDirectorByIdHandler;

                var director = handler(id);

                if (director == null)
                {
                    return Results.StatusCode(404);
                }

                return Results.Ok(director);
            });

            app.MapPost($"{basePath}/directors", async (HttpRequest req, Director director, HttpResponse res) =>
            {
                var result = req.Validate<Director>(director);

                if (!result.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.GetFormattedErrors());
                    return;
                }

                var handler = RouteHandlers.CreateDirectorHandler;

                var id = handler(director);

                res.StatusCode = 201;
                res.Headers["Location"] = "/" + id;
            });

            app.MapPut($"{basePath}/directors/{{id:int}}", async (int id, Director director, HttpContext ctx) =>
            {
                var result = ctx.Request.Validate<Director>(director);

                if (!result.IsValid)
                {
                    ctx.Response.StatusCode = 422;
                    await ctx.Response.Negotiate(result.GetFormattedErrors());
                    return;
                }

                var handler = RouteHandlers.UpdateDirectorHandler;

                try
                {
                    var success = handler(director);

                    if (!success)
                    {
                        ctx.Response.StatusCode = 400;
                        return;
                    }

                    ctx.Response.StatusCode = 204;
                }
                catch
                {
                    ctx.Response.StatusCode = 403;
                }
            });

            app.MapDelete($"{basePath}/directors/{{id:int}}", (int id) =>
            {
                var handler = RouteHandlers.DeleteDirectorHandler;

                try
                {
                    handler(id);

                    return Results.StatusCode(204);
                }
                catch (InvalidOperationException)
                {
                    return Results.StatusCode(403);
                }
            });
        }
    }
}
