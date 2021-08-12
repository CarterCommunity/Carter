namespace CarterSample.Features.CastMembers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using ValidatorOnlyProject;

    public class CastMemberModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/castmembers", (HttpRequest req, CastMember castMember) =>
            {
                var result = req.Validate<CastMember>(castMember);

                if (!result.IsValid)
                {
                    return Results.UnprocessableEntity(result.GetFormattedErrors());
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
}
