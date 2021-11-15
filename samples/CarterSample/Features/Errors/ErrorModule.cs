namespace CarterSample.Features.Errors;

public class ErrorModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/error", () => { throw new Exception("oops"); });

        app.MapGet("/errorhandler", (HttpContext ctx) =>
        {
            var error = string.Empty;
            var feature = ctx.Features.Get<IExceptionHandlerFeature>();
            if (feature != null)
            {
                if (feature.Error is ArgumentNullException)
                {
                    ctx.Response.StatusCode = 402;
                }

                error = feature.Error.ToString();
            }

            return $"There has been an error{Environment.NewLine}{error}";
        });
    }
}
