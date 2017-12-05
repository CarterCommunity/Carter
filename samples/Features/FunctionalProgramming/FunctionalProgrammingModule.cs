namespace Botwin.Samples
{
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

                var actor = handler?.Invoke();

                if (actor == null)
                {
                    res.StatusCode = 401;
                    return;
                }

                await res.AsJson(actor);
            });
            
            this.Get("/directors/{id:int}", async (req, res, routeData) =>
            {
                var handler = RouteHandlers.GetDirectorByIdHandler;

                var director = handler?.Invoke(routeData.As<int>("id"));

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
        }
    }
}
