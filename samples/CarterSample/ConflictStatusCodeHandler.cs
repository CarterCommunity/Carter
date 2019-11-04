namespace CarterSample
{
    using System.Threading.Tasks;
    using Carter;
    using Microsoft.AspNetCore.Http;

    public class ConflictStatusCodeHandler : IStatusCodeHandler
    {
        public bool CanHandle(int statusCode)
        {
            return statusCode == 409;
        }

        public Task Handle(HttpContext ctx)
        {
            return ctx.Response.WriteAsync("409");
        }
    }
}