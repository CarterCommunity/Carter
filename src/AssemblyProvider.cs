namespace Botwin
{
    using System.Reflection;
    public class AssemblyProvider : IAssemblyProvider
    {
        public Assembly GetAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}