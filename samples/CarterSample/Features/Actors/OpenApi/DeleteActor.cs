namespace CarterSample.Features.Actors.OpenApi
{
    using Carter.OpenApi;

    public class DeleteActor : RouteMetaData
    {
        public override string Description { get; } = "Delete an actor";

        public override RouteMetaDataResponse[] Responses { get; } = { new RouteMetaDataResponse { Code = 204, Description = "Deleted Actor" } };

        public override string Tag { get; } = "Actors";

        public override string OperationId { get; } = "Actors_DeleteActor";
    }
}
