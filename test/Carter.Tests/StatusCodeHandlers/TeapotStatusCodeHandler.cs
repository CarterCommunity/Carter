namespace Carter.Tests.StatusCodeHandlers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class TeapotStatusCodeHandler : IStatusCodeHandler
    {
        public bool CanHandle(int statusCode) => statusCode == 418;

        public async Task Handle(HttpContext ctx)
        {
            await ctx.Response.WriteAsync(" World");
        }
    }
}