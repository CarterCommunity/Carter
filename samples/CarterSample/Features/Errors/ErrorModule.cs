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
            this.Get("/error", (req, res) =>
            {
                throw new Exception("oops");
            });

            this.Get("/errorhandler", (req, res) =>
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
                return res.WriteAsync($"There has been an error{Environment.NewLine}{error}");
            });
        }
    }
}