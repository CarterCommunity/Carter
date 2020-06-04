namespace CarterSample.Features.Actors.OpenApi
{
    using Carter.OpenApi;
    using System.Collections.Generic;

    public class GetSimpleNullables : RouteMetaData
    {
        public override string Description { get; } = "Gets a SimpleNullable object";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200,
                Description = $"A list of {nameof(SimpleNullable)}s",
                Response = typeof(List<SimpleNullable>)
            }
        };

        public override string Tag { get; } = "SimpleNullable";

        public override string OperationId { get; } = "SimpleNullable";
    }
}
