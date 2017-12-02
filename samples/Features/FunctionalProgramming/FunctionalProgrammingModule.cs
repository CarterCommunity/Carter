namespace Botwin.Samples
{
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
        }
    }
}
