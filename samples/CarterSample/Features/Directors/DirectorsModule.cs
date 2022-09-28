namespace CarterSample.Features.Directors;

using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

public class DirectorsModule : CarterModule
{
    public DirectorsModule() : base("/directors")
    {
        // this.RequireAuthorization()
        //     .RequireHost("*:5000")
        //     .RequireCors("myorigins")
        //     .WithDescription("Directors API allows users to interact with the directors resources")
        //     .WithMetadata(new SuppressLinkGenerationMetadata())
        //     .WithName("Directors API")
        //     .WithSummary("An API resource for Directors")
        //     .WithTags("directors", "api")
        //     .WithDisplayName("Directors")
        //     .WithGroupName("directors-api")
        //     .WithCacheOutput("cachepolicyoutputname")
        //     //.DisableRateLimiting()
        //     .RequireRateLimiting("rateLimitingPolicyName")
        //     .IncludeInOpenApi();

        this.Before = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DirectorsModule>>();
            logger.LogDebug("Before");
            return null;
        };

        this.After = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DirectorsModule>>();
            logger.LogDebug("After");
        };
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (ILogger<DirectorsModule> logger) =>
        {
            logger.LogDebug("Route Hit");
            return "Directors";
        });

        app.MapPut<Person>("/",
            Results<NoContent, BadRequest>(IUpdatePersonCommand updatePersonCommand, Person person) =>
            {
                var success = updatePersonCommand.Execute(person);

                if (!success)
                {
                    return TypedResults.BadRequest();
                }

                return TypedResults.NoContent();
            });

        //app.MapPut<Person>("/", (Person person) => "PUT");

        app.MapPost("/", (HttpRequest req, Person person) =>
        {
            var asName = req.Query.As<string>("name");
            var asBar = req.Query.As<int>("bar");
            var asFoo = req.Query.AsMultiple<int>("foo");

            return "POST";
        });

        app.MapGet("/qs", (string name, int[] numbers) => name + string.Join(",", numbers));
    }
}

public class Person
{
    public string Name { get; set; }

    public int Age { get; set; }

    public int Id { get; set; }
}

public class PersonValidation : AbstractValidator<Person>
{
    public PersonValidation()
    {
        this.RuleFor(x => x.Name).NotEmpty();
        this.RuleFor(x => x.Age).NotEmpty();
    }
}

internal interface IUpdatePersonCommand
{
    bool Execute(Person person);
}

internal class UpdatePersonComand : IUpdatePersonCommand
{
    public bool Execute(Person person)
    {
        return true;
    }
}
