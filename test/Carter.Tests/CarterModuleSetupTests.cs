namespace Carter.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class CarterModuleSetupTests
    {
        private class ExampleModule : CarterModule
        {
            public ExampleModule()
            {
                this.Post("/foo", ctx => Task.CompletedTask);
                this.Post("/bar", ctx => Task.CompletedTask);
            }
        }

        private class ExampleModuleWithConflictingRoutes : CarterModule
        {
            public ExampleModuleWithConflictingRoutes()
            {
                this.Post("/foo", ctx => Task.CompletedTask);
                this.Post("/Foo", ctx => Task.CompletedTask);
            }
        }

        [Fact]
        public void Module_attempts_to_register_conflicting_routes()
        {
            Assert.Throws<ArgumentException>(() => { new ExampleModuleWithConflictingRoutes(); });
        }

        [Fact]
        public void Module_registers_two_routes()
        {
            var m = new ExampleModule();
            Assert.Equal(2, m.Routes.Count);
        }
    }
}
