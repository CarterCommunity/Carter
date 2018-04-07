namespace CarterSample.Features.FunctionalProgramming
{
    using System;
    using System.Threading.Tasks;
    using Carter;
    using Carter.ModelBinding;
    using Carter.Request;
    using Carter.Response;

    public class FunctionalProgrammingModule : CarterModule
    {
        public FunctionalProgrammingModule() : base("/functional")
        {
            this.Get("/directors", async (req, res, routeData) =>
            {
                var handler = RouteHandlers.ListDirectorsHandler;

                var directors = handler();

                if (directors == null)
                {
                    res.StatusCode = 403;
                    return;
                }

                await res.AsJson(directors);
            });

            this.Get("/directors/{id:int}", async (req, res, routeData) =>
            {
                var handler = RouteHandlers.GetDirectorByIdHandler;

                var director = handler(routeData.As<int>("id"));

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

                var id = handler(result.Data);

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
                    var success = handler(result.Data);

                    if (!success)
                    {
                        res.StatusCode = 400;
                        return;
                    }

                    res.StatusCode = 204;
                }
                catch
                {
                    res.StatusCode = 403;
                }
            });

            this.Delete("/directors/{id:int}", (req, res, routeData) =>
            {
                var handler = RouteHandlers.DeleteDirectorHandler;

                try
                {
                    handler(routeData.As<int>("id"));

                    res.StatusCode = 204;
                    return Task.CompletedTask;
                }
                catch (InvalidOperationException)
                {
                    res.StatusCode = 403;
                    return Task.CompletedTask;
                }
            });
        }
    }
}
