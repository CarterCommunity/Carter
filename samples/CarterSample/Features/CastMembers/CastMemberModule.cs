namespace CarterSample.Features.CastMembers
{
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using CarterSample.Features.CastMembers.OpenApi;
    using Microsoft.AspNetCore.Http;
    using ValidatorOnlyProject;

    public class CastMemberModule : CarterModule
    {
        public CastMemberModule()
        {
            this.Post("/castmembers", async (req, res, routeData) =>
            {
                var result = await req.BindAndValidate<CastMember>();

                if (!result.ValidationResult.IsValid)
                {
                    await res.AsJson(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                await res.WriteAsync("OK");
            });

            this.Get<GetCastMembers>("/castmembers", (request, response, routeData) =>
            {
                var castMembers = new[] { new CastMember { Name = "Samuel L Jackson" }, new CastMember { Name = "John Travolta" } };

                return response.AsJson(castMembers);
            });
        }
    }
}
