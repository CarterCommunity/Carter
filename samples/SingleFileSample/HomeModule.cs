namespace SingleFileSample;

using Carter;
using Carter.ModelBinding;
using ValidatorOnlyProject;

public class HomeModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Hello from a single file Carter app");

        // Greeting and its validator live in this assembly
        app.MapPost("/greetings", (HttpRequest req, Greeting greeting) =>
        {
            var result = req.Validate(greeting);

            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.GetValidationProblems(), statusCode: 422);
            }

            return Results.Ok($"Hello {greeting.Name}");
        });

        // CastMember and its validator live in the referenced ValidatorOnlyProject assembly
        app.MapPost("/castmembers", async (HttpRequest req, CastMember castMember) =>
        {
            var result = await req.ValidateAsync(castMember);

            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.GetValidationProblems(), statusCode: 422);
            }

            return Results.Ok($"Hello {castMember.Name}");
        });
    }
}
