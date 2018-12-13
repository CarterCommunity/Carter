namespace CarterSample.Features.Actors
{
    using System;
    using Carter;

    public class DeleteActor : RouteMetaData
    {
        public override string Description { get; } = "Delete an actor";

        public override RouteMetaDataResponse[] Responses { get; } = { new RouteMetaDataResponse { Code = 204, Description = "Deleted Actor" } };

        public override string Tag { get; } = "Actors";
    }
}
