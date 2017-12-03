namespace Botwin.Samples.CreateDirector
{
    public class CreateDirectorRoute
    {
        public delegate int CreateDirector(Director director);

        public static int Handle(Director director, CreateDirector createDirector)
        {
            return createDirector(director);
        }
    }
}
