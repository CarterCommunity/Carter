namespace Carter.Tests
{
    using System;
    using System.Linq;
    using Carter.Tests.Modelbinding;
    using Xunit;

    public class TypeExtensionTests
    {
        [Fact]
        public void MustDeriveFrom_TypesDerivingFrom_WontThrow()
        {
            var types = new[] { typeof(TestModule), typeof(BindModule) }.ToArray();
            types.MustDeriveFrom<CarterModule>();
        }

        [Fact]
        public void MustDeriveFrom_TypesDontDeriveFrom_WillThrow()
        {
            var types = new[] { typeof(CarterModule), typeof(DateTime) };
            Assert.Throws<ArgumentException>(() => types.MustDeriveFrom<CarterModule>());
        }
    }
}
