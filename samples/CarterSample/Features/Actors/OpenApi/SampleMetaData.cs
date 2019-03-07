namespace CarterSample.Features.Actors
{
    using System;
    using Carter.OpenApi;

    public class SampleMetaData : RouteMetaData
    {
        public override Type Request { get; } = typeof(Foo);

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = 200, Response = typeof(PagedItems<Foo>) }
        };
    }
}