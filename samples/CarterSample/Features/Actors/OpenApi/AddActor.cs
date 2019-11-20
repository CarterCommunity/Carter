namespace CarterSample.Features.Actors.OpenApi
{
    using System;
    using Carter.OpenApi;

    public class AddActor : RouteMetaData
    {
        public override string Description { get; } = "Create an actor in the system";

        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(Actor),
            }
        };

        public override RouteMetaDataResponse[] Responses { get; } = { new RouteMetaDataResponse { Code = 201, Description = "Created Actors" } };

        public override string Tag { get; } = "Actors";

        public override string OperationId { get; } = "Actors_AddActor";
    }
}
