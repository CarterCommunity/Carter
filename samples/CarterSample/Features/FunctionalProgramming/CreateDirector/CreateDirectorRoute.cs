namespace CarterSample.Features.FunctionalProgramming.CreateDirector
{
    public class CreateDirectorRoute
    {
        public delegate int CreateDirectorHandler(Director director);

        public delegate int CreateDirector(Director director);
        
        public static int Handle(Director director, CreateDirector createDirector)
        {
            return createDirector(director);
        }
    }
}
