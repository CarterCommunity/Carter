using System;
using System.Reflection;

namespace Botwin.Tests
{
    public class TestAssemblyProvider : IAssemblyProvider
    {
        public Assembly GetAssembly()
        {
            return typeof(TestModule).GetTypeInfo().Assembly;
        }
    }
}