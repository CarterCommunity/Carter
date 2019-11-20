namespace CarterSample.Features.Actors
{
    using System;
    using Carter.OpenApi;

    public class NoValidatorMetaData : RouteMetaData
    {
        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(Foo),
            }
        };
    }
}