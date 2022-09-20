public class HomeModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("/", () => "Hello from Carter!");
}
