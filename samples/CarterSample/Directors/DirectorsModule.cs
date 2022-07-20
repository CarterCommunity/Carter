namespace CarterSample.Directors;

using System.Diagnostics.CodeAnalysis;
using Carter.OpenApi;
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

    private static async Task<Results<NoContent, NotFound, Ok<string>>> Handler(HttpContext context)
    {
        return TypedResults.Ok("");
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

interface IUpdatePersonCommand
{
    bool Execute(Person person);
}

public static class RouteExtensions
{
    public static RouteHandlerBuilder MapPost<T>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        return endpoints.MapPost(pattern, handler).AddRouteHandlerFilter(async (context, next) =>
        {
            var tdparam = context.GetArgument<T>(0);

            var result = context.HttpContext.Request.Validate(tdparam);
            if (!result.IsValid)
            {
                var problemDetailsErrors = new Dictionary<string, object>()
                    { { "errors", result.GetFormattedErrors() } };
                return Results.Problem(statusCode: 422, extensions: problemDetailsErrors);
            }

            return await next(context);
        });
    }

    public static RouteHandlerBuilder MapPut<T>(
        this IEndpointRouteBuilder endpoints,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        return endpoints.MapPut(pattern, handler).AddRouteHandlerFilter(async (context, next) =>
        {
            var tdparam = context.GetArgument<T>(0);

            var result = context.HttpContext.Request.Validate(tdparam);
            if (!result.IsValid)
            {
                var problemDetailsErrors = new Dictionary<string, object>()
                    { { "errors", result.GetFormattedErrors() } };
                return Results.Problem(statusCode: 422, extensions: problemDetailsErrors);
            }

            return await next(context);
        });
    }
}
