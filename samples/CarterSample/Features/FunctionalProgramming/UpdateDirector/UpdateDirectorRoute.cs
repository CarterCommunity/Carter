namespace CarterSample.Features.FunctionalProgramming.UpdateDirector
{
    using System;

    public class UpdateDirectorRoute
    {
        public delegate bool UpdateDirectorHandler(Director updatedDirector);

        public delegate int UpdateDirectorInDB(Director director);
        public delegate bool UserAllowed();
        
        public static bool Handle(Director updatedDirector, UpdateDirectorInDB updateDirector, UserAllowed userAllowed )
        {
            var currentUserAllowed = userAllowed();
            if (!currentUserAllowed)
            {
                throw new InvalidOperationException();
            }

            var result =  updateDirector(updatedDirector);
            return result > 0;
        }
    }
}
