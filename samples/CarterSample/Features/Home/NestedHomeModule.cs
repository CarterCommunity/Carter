namespace CarterSample.Features.Home;

public static class NestedHomeModule
{
    public class HomeModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/nestedHome", (HttpResponse res) =>
            {
                res.StatusCode = 409;
                return Results.Text("There's no place like 127.0.0.1");
            });
        }
    }
}
