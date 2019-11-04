namespace Carter.Tests
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class NoOpStatusCodeHandler : IStatusCodeHandler
    {
        public bool CanHandle(int statusCode) => statusCode == 200;

        public Task Handle(HttpContext ctx) => Task.CompletedTask;
    }
}