namespace CarterSample.Features.Actors
{
    using Carter.OpenApi;

    public class SampleMetaData : RouteMetaData
    {
        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = 200, Response = typeof(PagedItems<Foo>) }
        };
    }
}