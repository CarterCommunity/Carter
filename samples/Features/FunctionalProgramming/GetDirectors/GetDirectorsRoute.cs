namespace Botwin.Samples
{
    using System.Collections.Generic;

    public static class GetDirectorsRoute
    {
        public delegate IEnumerable<Director> ListDirectors();
        public delegate bool UserAllowed();
        
        public static IEnumerable<Director> Handle(ListDirectors listDirectors, UserAllowed userAllowed)
        {
            var currentUserAllowed = userAllowed();
            if (!currentUserAllowed)
            {
                return null;
            }

            var directors = listDirectors();
            return directors;
        }
    }
}