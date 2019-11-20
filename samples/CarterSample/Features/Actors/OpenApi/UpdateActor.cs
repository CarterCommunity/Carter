namespace CarterSample.Features.Actors.OpenApi
{
    using System;
    using Carter.OpenApi;

    public class UpdateActor : RouteMetaData
    {
        public override string Tag { get; } = "Actors";
        
        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(Actor),
            }
        };

        public override string Description { get; } = "Update an existing actor";

        public override RouteMetaDataResponse[] Responses { get; } = { new RouteMetaDataResponse { Code = 204, Description = "Updated Actor" } };
        
        public override string OperationId { get; } = "Actors_UpdateActor";

    }
}
