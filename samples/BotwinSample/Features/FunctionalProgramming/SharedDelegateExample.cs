namespace Botwin.Samples
{
    public delegate bool SharedDelegateExample();

    public class SharedImplementations
    {
        public static bool SharedImplementation()
        {
            return false;
        }
    }
}