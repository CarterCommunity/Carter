namespace Carter.Tests
{
    using System;
    using System.Net;
    using Carter.Request;
    using Carter.Response;

    public class SameRouteModule : CarterModule
    {
        public SameRouteModule()
        {
            Put("/sametest/{id:int}/blah/{otherid1:guid}", async (request, response) =>
            {
                var id = request.RouteValues.As<int>("id");
                var otherid = request.RouteValues.As<Guid>("otherid1");

                response.StatusCode = (int) HttpStatusCode.OK;
                await response.AsJson(new
                {
                    Id = id,
                    Other = otherid,
                    Method = "Put"
                });
            });

            Get("/sametest/{id:int}/blah/{otherid2:guid}", async (request, response) =>
            {
                var id = request.RouteValues.As<int>("id");
                var otherid = request.RouteValues.As<Guid>("otherid2");

                response.StatusCode = (int) HttpStatusCode.OK;
                await response.AsJson(new
                {
                    Id = id,
                    Other = otherid,
                    Method = "Get"
                });
            });
        }
    }
}
