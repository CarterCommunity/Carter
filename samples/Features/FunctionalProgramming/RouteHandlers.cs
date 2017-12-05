namespace Botwin.Samples
{
    using System;
    using Botwin.Samples.CreateDirector;
    using Botwin.Samples.GetDirectorById;
    using BotwinSample;

    public static class RouteHandlers
    {
        private static GetDirectorsRoute.ListDirectors listDirectors;

        private static GetDirectorByIdRoute.GetDirectorById getDirectorById;

        private static CreateDirectorRoute.CreateDirector createDirector;

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

        public static CreateDirectorRoute.CreateDirector CreateDirectorHandler
        {
            get { return createDirector ?? (director => CreateDirectorRoute.Handle(director, newDirector =>
            {
                //Create database connection here and store in the database
                
                /*
                using(var conn = new NpgsqlConnection(AppConfiguration.ConnectionString))
                {
                   return conn.Execute("insert into director (name, age, dob) values (@name, @age, @dob)",director);
                }
                */
                return 1123;
            })); }
            set => createDirector = value;
        }
    }
}
