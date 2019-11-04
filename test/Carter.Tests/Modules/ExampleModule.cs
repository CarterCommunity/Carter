namespace Carter.Tests.Modules
{
    using System.Threading.Tasks;

    public class ExampleModule : CarterModule
    {
        public ExampleModule()
        {
            this.Post("/foo", ctx => Task.CompletedTask);
            this.Post("/bar", ctx => Task.CompletedTask);
        }
    }
}