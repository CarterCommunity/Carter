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
                var data = Composition.ComposeFunctionalHandler();

                var actor = data();

                await res.Negotiate(actor);
            });
        }
    }

    public class Composition
    {
        public static Func<Actor> ComposeFunctionalHandler()
        {
            return () => FunctionalRoute.Handle(() =>
            {
                Console.WriteLine($"Getting sql connection from settings {AppConfiguration.ConnectionString} as an example of how you'd get app settings");
                return new[] { new Actor() };
            }, () => true);
        }
    }

    public class AppConfiguration
    {
        public static string ConnectionString { get; set; }
    }

    public static class FunctionalRoute
    {
        public static Actor Handle(Func<IEnumerable<Actor>> getActors, Func<bool> userAllowed)
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
