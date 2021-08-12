namespace CarterSample.Features.Actors
{
    using System.IO;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Request;
    using Carter.Response;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class ActorsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/actors", (IActorProvider actorProvider, HttpResponse res) =>
            {
                var people = actorProvider.Get();
                return people;
            });

             app.MapGet("/actors/{id:int}", (int id, IActorProvider actorProvider, HttpResponse res) =>
            {
                var person = actorProvider.Get(id);
                return res.Negotiate(person);
            });

            app.MapPut("/actors/{id:int}", async (HttpRequest req, Actor actor, HttpResponse res) =>
            {
                var result = req.Validate<Actor>(actor);

                if (!result.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.GetFormattedErrors());
                    return;
                }

                //Update the user in your database

                res.StatusCode = 204;
            });

            app.MapPost("/actors", async (HttpContext ctx, Actor actor) =>
            {
                var result = ctx.Request.Validate<Actor>(actor);

                if (!result.IsValid)
                {
                    ctx.Response.StatusCode = 422;
                    await ctx.Response.Negotiate(result.GetFormattedErrors());
                    return;
                }

                //Save the user in your database

                ctx.Response.StatusCode = 201;
                await ctx.Response.Negotiate(actor);
            });

            app.MapDelete("/actors/{id:int}", (int id, IActorProvider actorProvider, HttpResponse res) =>
            {
                actorProvider.Delete(id);
                return Results.StatusCode(204);
            });

            app.MapGet("/actors/download", async (HttpResponse response) =>
            {
                using (var video = new FileStream("earth.mp4", FileMode.Open)) //24406813
                {
                    await response.FromStream(video, "video/mp4");
                }
            });

            app.MapGet("/empty", () => Task.CompletedTask);

            app.MapGet("/actors/sample", () => Task.CompletedTask);

            app.MapPost("/actors/sample", () => Task.CompletedTask);

            app.MapGet("/nullable", () => Task.CompletedTask); 
        }
    }
}
