namespace CarterSample.Features.Directors;

using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

public class DirectorsModule : CarterModule
{
    public DirectorsModule() : base("/directors")
    {
        this.RequireAuthorization()
            .RequireHost("*:5000")
            .RequireCors("myorigins")
            .WithDescription("Directors API allows users to interact with the directors resources")
            .WithMetadata(new SuppressLinkGenerationMetadata())
            .WithName("Directors API")
            .WithSummary("An API resource for Directors")
            .WithTags("directors", "api")
            .WithDisplayName("Directors")
            .WithGroupName("directors-api")
            .IncludeInOpenApi();

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