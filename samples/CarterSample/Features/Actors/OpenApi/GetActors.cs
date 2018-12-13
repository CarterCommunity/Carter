namespace CarterSample.Features.Actors
{
    using System;
    using System.Collections.Generic;
    using Carter;

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
    }
}
