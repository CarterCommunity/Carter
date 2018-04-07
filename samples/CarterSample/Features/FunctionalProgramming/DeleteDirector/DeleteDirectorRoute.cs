namespace CarterSample.Features.FunctionalProgramming.DeleteDirector
{
    using System;

    public class DeleteDirectorRoute
    {
        public delegate int DeleteDirectorById(int id);

        public delegate void DeleteDirectorHandler(int id);
        
        public static void Handle(int dirId, DeleteDirectorById deleteDirectorById, Func<bool> userAllowed)
        {
            if (!userAllowed())
            {
                throw new InvalidOperationException();
            }

            deleteDirectorById(dirId);
        }
    }
}