namespace Botwin.Samples
{
    using Botwin.Response;

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
        }
    }
}
