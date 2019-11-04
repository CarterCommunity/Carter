namespace CarterSample.Features.CastMembers.OpenApi
{
    using System.Collections.Generic;
    using Carter.OpenApi;
    using ValidatorOnlyProject;

    public class GetCastMembers : RouteMetaData
    {
        public override string Description { get; } = "Returns a list of actors";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200,
                Description = $"A list of {nameof(CastMember)}s",
                Response = typeof(IEnumerable<CastMember>)
            }
        };

        public override string Tag { get; } = "CastMembers";

        public override string OperationId { get; } = "CastMembers_GetCastMembers";
    }
}
