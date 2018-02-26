namespace BotwinSample.Features.Directors
{
    using Botwin;
    using Botwin.ModelBinding;
    using Botwin.Response;
    using Microsoft.AspNetCore.Http;
    using ValidatorOnlyProject;

    public class CastMemberModule : BotwinModule
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
