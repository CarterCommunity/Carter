namespace Botwin.Samples
{
    using System;
    using Botwin.Samples.GetDirectorById;
    using BotwinSample;

    public static class RouteHandlers
    {
        private static GetDirectorsRoute.ListDirectors listDirectors;
        private static GetDirectorByIdRoute.GetDirectorById getDirectorById;

        public static GetDirectorsRoute.ListDirectors ListDirectorsHandler
        {
            get
            {
                return listDirectors ?? (() => GetDirectorsRoute.Handle(listDirectors: () =>
                {
                    Console.WriteLine($"Getting sql connection from settings {AppConfiguration.ConnectionString} as an example of how you'd get app settings");
                    return new[] { new Director() };
                }, userAllowed: () => true));
            }

            set => listDirectors = value;
        }

        public static GetDirectorByIdRoute.GetDirectorById GetDirectorByIdHandler
        {
            get { return getDirectorById ?? (dirId => GetDirectorByIdRoute.Handle(dirId, id => new Director(), () => true)); }
            set => getDirectorById = value;
        }
    }
}
