namespace CarterSample.Features.CastMembers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
            this.Post("/castmembers", async (req, res) =>
            {
                var result = await req.BindAndValidate<CastMember>();

                if (!result.ValidationResult.IsValid)
                {
                    await res.AsJson(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                await res.WriteAsync("OK");
            });

            this.Get<GetCastMembers>("/castmembers", async(request, response) =>
            {
                var castMembers = new[] { new CastMember { Name = "Samuel L Jackson" }, new CastMember { Name = "John Travolta" } };

                await response.AsJson(castMembers);
            });
        }
    }
}
