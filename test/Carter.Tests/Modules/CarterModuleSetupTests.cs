namespace Carter.Tests.Modules
{
    using System;
    using Xunit;

    public class CarterModuleSetupTests
    {
        [Fact]
        public void Module_attempts_to_register_conflicting_routes()
        {
            var ex = Assert.Throws<ArgumentException>(() => { new ExampleModuleWithConflictingRoutes(); });
            Assert.Equal("An item with the same key has already been added. Key: (POST, /Foo)", ex.Message);
        }

        [Fact]
        public void Module_registers_two_routes()
        {
            var m = new ExampleModule();
            Assert.Equal(2, m.Routes.Count);
        }
    }
}
