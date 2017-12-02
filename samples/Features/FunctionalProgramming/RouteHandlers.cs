namespace Botwin.Samples
{
    using System;
    using BotwinSample;

    public static class RouteHandlers
    {
        private static GetDirectorsRoute.ListDirectors func;

        public static GetDirectorsRoute.ListDirectors ListDirectorsHandler
        {
            get
            {
                return func ?? (() => GetDirectorsRoute.Handle(listDirectors: () =>
                {
                    Console.WriteLine($"Getting sql connection from settings {AppConfiguration.ConnectionString} as an example of how you'd get app settings");
                    return new[] { new Director() };
                }, userAllowed: () => true));
            }

            set => func = value;
        }
    }
}
