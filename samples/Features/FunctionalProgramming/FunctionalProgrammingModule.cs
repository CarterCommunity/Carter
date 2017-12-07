namespace Botwin.Samples
{
    using System;
    using System.Threading.Tasks;
    using Botwin.ModelBinding;
    using Botwin.Request;
    using Botwin.Response;
    using Microsoft.AspNetCore.Routing;

    public class FunctionalProgrammingModule : BotwinModule
    {
        public FunctionalProgrammingModule() : base("/functional")
        {
            this.Get("/directors", async (req, res, routeData) =>
            {
                var handler = RouteHandlers.ListDirectorsHandler;

                var actor = handler.Invoke();

                if (actor == null)
                {
                    res.StatusCode = 403;
                    return;
                }

                await res.AsJson(actor);
            });

            this.Get("/directors/{id:int}", async (req, res, routeData) =>
            {
                var handler = RouteHandlers.GetDirectorByIdHandler;

                var director = handler.Invoke(routeData.As<int>("id"));

                if (director == null)
                {
                    res.StatusCode = 404;
                    return;
                }

                await res.AsJson(director);
            });

            this.Post("/directors", async (req, res, routeData) =>
            {
                var result = req.BindAndValidate<Director>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                var handler = RouteHandlers.CreateDirectorHandler;

                var id = handler.Invoke(result.Data);

                res.StatusCode = 201;
                res.Headers["Location"] = "/" + id;
            });

            this.Put("/directors/{id:int}", async (req, res, routeData) =>
            {
                var result = req.BindAndValidate<Director>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                var handler = RouteHandlers.UpdateDirectorHandler;

                try
                {
                    var success = handler.Invoke(result.Data);

                    if (!success)
                    {
                        res.StatusCode = 400;
                        return;
                    }

                    res.StatusCode = 204;
                }
                catch (Exception e)
                {
                    res.StatusCode = 403;
                }
            });

            this.Delete("/directors/{id:int}", (req, res, routeData) =>
            {
                var handler = RouteHandlers.DeleteDirectorHandler;

                try
                {
                    handler.Invoke(routeData.As<int>("id"));

                    res.StatusCode = 204;
                    return Task.CompletedTask;
                }
                catch (InvalidOperationException e)
                {
                    res.StatusCode = 403;
                    return Task.CompletedTask;
                }
            });
        }
    }
}
