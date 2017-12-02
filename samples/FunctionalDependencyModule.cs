namespace Botwin.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Botwin.Response;

    public class FunctionalDependencyModule : BotwinModule
    {
        public FunctionalDependencyModule()
        {
            this.Get("/functional", async (req, res, routeData) =>
            {
                var handler = RouteHandlers.GetActorHandler;

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

    public class RouteHandlers
    {
        public delegate Actor GetActor();

        private static GetActor func;

        public static GetActor GetActorHandler
        {
            get
            {
                return func ?? (() => FunctionalRoute.Handle(getActors: () =>
                     {
                         Console.WriteLine($"Getting sql connection from settings {AppConfiguration.ConnectionString} as an example of how you'd get app settings");
                         return new[] { new Actor() };
                     }, userAllowed: () => true));
            }

            set
            {
                func = value;
            }
        }
    }
    public class AppConfiguration
    {
        public static string ConnectionString { get; set; }
    }

    public static class FunctionalRoute
    {
        public delegate IEnumerable<Actor> GetActors();
        public delegate bool UserAllowed();

        public static Actor Handle(GetActors getActors, UserAllowed userAllowed)
        {
            var currentUserAllowed = userAllowed();
            if (!currentUserAllowed)
            {
                return null;
            }

            var actors = getActors();
            return actors.FirstOrDefault();
        }
    }
}
