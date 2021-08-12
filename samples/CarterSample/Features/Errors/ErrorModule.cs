namespace CarterSample.Features.Errors
{
    using System;
    using Carter;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class ErrorModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/error", () =>
            {
                throw new Exception("oops");
            });

            app.MapGet("/errorhandler", (HttpContext ctx) =>
            {
                string error = string.Empty;
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
}