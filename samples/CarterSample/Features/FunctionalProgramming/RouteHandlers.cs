namespace CarterSample.Features.FunctionalProgramming
{
    using System;
    using CarterSample.Features.FunctionalProgramming.CreateDirector;
    using CarterSample.Features.FunctionalProgramming.DeleteDirector;
    using CarterSample.Features.FunctionalProgramming.GetDirectorById;
    using CarterSample.Features.FunctionalProgramming.ListDirectors;
    using CarterSample.Features.FunctionalProgramming.UpdateDirector;

    public static class RouteHandlers
    {
        private static ListDirectorsRoute.ListDirectorsHandler listDirectors;
        private static GetDirectorByIdRoute.GetDirectorByIdHandler getDirectorById;
        private static CreateDirectorRoute.CreateDirectorHandler createDirector;
        private static UpdateDirectorRoute.UpdateDirectorHandler updateDirectorHandler;
        private static DeleteDirectorRoute.DeleteDirectorHandler deleteDirectorHandler;

        public static ListDirectorsRoute.ListDirectorsHandler ListDirectorsHandler
        {
            get
            {
                return listDirectors ?? (() => ListDirectorsRoute.Handle(listDirectors: () =>
                {
                    Console.WriteLine($"Getting sql connection from settings {AppConfiguration.ConnectionString} as an example of how you'd get app settings");
                    return new[] { new Director() };
                }, userAllowed: () => true, sharedDelegateExample: SharedImplementations.SharedImplementation));
            }

            set => listDirectors = value;
        }

        public static GetDirectorByIdRoute.GetDirectorByIdHandler GetDirectorByIdHandler
        {
            get { return getDirectorById ?? (dirId => GetDirectorByIdRoute.Handle(dirId, id => new Director(), () => true)); }
            set => getDirectorById = value;
        }

        public static CreateDirectorRoute.CreateDirectorHandler CreateDirectorHandler
        {
            get
            {
                return createDirector ?? (director => CreateDirectorRoute.Handle(director, newDirector =>
                {
                    //Create database connection here and store in the database

                    /*
                    using(var conn = new NpgsqlConnection(AppConfiguration.ConnectionString))
                    {
                       return conn.Execute("insert into director (name, age, dob) values (@name, @age, @dob)",director);
                    }
                    */
                    return 1123;
                }));
            }
            set => createDirector = value;
        }

        public static UpdateDirectorRoute.UpdateDirectorHandler UpdateDirectorHandler
        {
            get { return updateDirectorHandler ?? (director => UpdateDirectorRoute.Handle(director, director1 => 1, () => true)); }
            set => updateDirectorHandler = value;
        }

        public static DeleteDirectorRoute.DeleteDirectorHandler DeleteDirectorHandler
        {
            get { return deleteDirectorHandler ?? (dirId => DeleteDirectorRoute.Handle(dirId, directorId => 1, () => true)); }
            set => deleteDirectorHandler = value;
        }
    }
}
