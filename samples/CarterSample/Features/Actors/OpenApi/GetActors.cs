namespace CarterSample.Features.Actors.OpenApi
{
    using System.Collections.Generic;
    using Carter.OpenApi;

    public class GetActors : RouteMetaData
    {
        public override string Description { get; } = "Returns a list of actors";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200,
                Description = $"A list of {nameof(Actor)}s",
                Response = typeof(IEnumerable<Actor>)
            }
        };

        public override string Tag { get; } = "Actors";

        public override string OperationId { get; } = "Actors_GetActors";

        public override string SecuritySchema { get; set; } = "ApiKey";

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            new QueryStringParameter { Name = "Offset", Description = "How man items to offset from the start of the list", Type = typeof(int) },
            new QueryStringParameter { Name = "Size", Description = "How many items to return at one time. MAX:20", Type = typeof(int) }
        };
    }
}
