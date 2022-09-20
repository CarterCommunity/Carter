namespace Carter.ResponseNegotiators.Newtonsoft.Tests
{
    using Carter.Response;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class NewtonsoftJsonResponseNegotiatorModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/negotiate", (HttpResponse res) => res.Negotiate(new { FirstName = "Jim" }));
        }
    }
}
