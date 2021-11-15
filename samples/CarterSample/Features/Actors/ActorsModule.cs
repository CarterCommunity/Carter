namespace CarterSample.Features.Actors;

public class ActorsModule : ICarterModule
{
    public static IEnumerable<Actor> ActorsDatabase = new[]
        { new Actor { Name = "Brad Pitt", Id = 1, Age = 51 }, new Actor { Name = "Jason Statham", Id = 2, Age = 43 } };

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/actors", (IGetActorsQuery getActorsQuery, HttpResponse res) =>
        {
            var people = getActorsQuery.Execute();
            return people;
        });

        app.MapGet("/actors/{id:int}", (int id, IGetActorByIdQuery getActorByIdQuery, HttpResponse res) =>
        {
            var person = getActorByIdQuery.Execute(id);
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

        app.MapDelete("/actors/{id:int}", (int id, IDeleteActorCommand deleteActorCommand, HttpResponse res) =>
        {
            deleteActorCommand.Execute(id);
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
