namespace Carter.Tests
{
    using System;
    using System.Linq;
    using Carter.Tests.ModelBinding;
    using Carter.Tests.StreamTests;
    using Xunit;

    public class TypeExtensionTests
    {
        [Fact]
        public void MustDeriveFrom_TypesDerivingFrom_WontThrow()
        {
            var types = new[] { typeof(TestModule), typeof(StreamModule) }.ToArray();
            types.MustDeriveFrom<ICarterModule>();
        }

        [Fact]
        public void MustDeriveFrom_TypesDontDeriveFrom_WillThrow()
        {
            var types = new[] { typeof(ICarterModule), typeof(DateTime) };
            Assert.Throws<ArgumentException>(() => types.MustDeriveFrom<ICarterModule>());
        }
    }
}
