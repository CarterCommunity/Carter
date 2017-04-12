namespace Botwin.Samples
{
    using System;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;

    public class ErrorModule : BotwinModule
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
                    error = feature.Error.ToString();
                }
                await res.WriteAsync($"There has been an error{Environment.NewLine}{error}");
            });
        }
    }
}