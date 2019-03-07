namespace CarterSample.Features.Actors
{
    using System;
    using Carter.OpenApi;

    public class NoValidatorMetaData : RouteMetaData
    {
        public override Type Request { get; } = typeof(Foo);
    }
}