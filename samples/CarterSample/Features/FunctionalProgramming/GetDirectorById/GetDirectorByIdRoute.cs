namespace CarterSample.Features.FunctionalProgramming.GetDirectorById
{
    public class GetDirectorByIdRoute
    {
        public delegate Director GetDirectorByIdHandler(int id);
        
        public delegate Director GetDirectorById(int id);

        public delegate bool UserAllowed();
        
        public static Director Handle(int id, GetDirectorById getDirectorById, UserAllowed userAllowed)
        {
            var currentUserAllowed = userAllowed();
            if (!currentUserAllowed)
            {
                return null;
            }

            var director = getDirectorById(id);
            return director;
        }
    }
}
