namespace CarterSample.Features.Actors.OpenApi
{
    using Carter.OpenApi;

    public class GetActorById : RouteMetaData
    {
        public override string Description { get; } = "Gets an actor by it's id";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200, Description = $"An {nameof(Actor)}",
                Response = typeof(Actor)
            },
            new RouteMetaDataResponse
            {
                Code = 404,
                Description = $"{nameof(Actor)} not found"
            }
        };

        public override string Tag { get; } = "Actors";

        public override string OperationId { get; } = "Actors_GetActorById";
    }
}
