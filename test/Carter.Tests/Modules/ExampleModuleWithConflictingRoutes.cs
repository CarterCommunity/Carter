namespace Carter.Tests.Modules
{
    using System.Threading.Tasks;

    public class ExampleModuleWithConflictingRoutes : CarterModule
    {
        public ExampleModuleWithConflictingRoutes()
        {
            this.Post("/foo", ctx => Task.CompletedTask);
            this.Post("/Foo", ctx => Task.CompletedTask);
        }
    }
}