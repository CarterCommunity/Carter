namespace CarterSample.Features.Errors
{
    using System;
    using Carter;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;

    public class ErrorModule : CarterModule
    {
        public ErrorModule()
        {
            this.Get("/error", (req, res, routeData) =>
            {
                throw new Exception("oops");
            });

            this.Get("/errorhandler", async (req, res, routeData) =>
            {
                string error = string.Empty;
                var feature = req.HttpContext.Features.Get<IExceptionHandlerFeature>();
                if (feature != null)
                {
                    if (feature.Error is ArgumentNullException)
                    {
                        res.StatusCode = 402;
                    }
                    error = feature.Error.ToString();
                }
                await res.WriteAsync($"There has been an error{Environment.NewLine}{error}");
            });
        }
    }
}