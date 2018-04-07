namespace CarterSample.Features.CastMembers
{
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Microsoft.AspNetCore.Http;
    using ValidatorOnlyProject;

    public class CastMemberModule : CarterModule
    {
        public CastMemberModule()
        {
            this.Post("/castmembers", async (req, res, routeData) =>
            {
                var result = req.BindAndValidate<CastMember>();

                if (!result.ValidationResult.IsValid)
                {
                    await res.AsJson(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                await res.WriteAsync("OK");
            });
        }
    }
}
