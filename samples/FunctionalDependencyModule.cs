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
                var handler = Composition.FunctionalHandler;

                var actor = handler?.Invoke();

                await res.Negotiate(actor);
            });
        }
    }

    public class Composition
    {
        public delegate Actor GetActor();

        private static GetActor func;
        
        public static GetActor FunctionalHandler
        {
            get
            {
                return func ?? (() => FunctionalRoute.Handle(() =>
                    {
                        Console.WriteLine($"Getting sql connection from settings {AppConfiguration.ConnectionString} as an example of how you'd get app settings");
                        return new[] { new Actor() };
                    }, () => true));
            }

            set
            {
                func = value;
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
}