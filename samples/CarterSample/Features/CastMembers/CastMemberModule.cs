namespace CarterSample.Features.CastMembers;

public class CastMemberModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/castmembers", (HttpRequest req, CastMember castMember) =>
        {
            var result = req.Validate<CastMember>(castMember);

            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.GetValidationProblems(), statusCode: 422);
            }

            return Results.Ok("OK");
        });

        app.MapGet("/castmembers", () =>
        {
            var castMembers = new[] { new CastMember { Name = "Samuel L Jackson" }, new CastMember { Name = "John Travolta" } };

            return castMembers;
        });
    }
}