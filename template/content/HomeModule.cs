namespace CarterTemplate
{
    using Carter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    public class HomeModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("/", () => "Hello from Carter!");
    }
}
