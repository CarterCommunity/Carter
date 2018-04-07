namespace Carter.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class StatusCodeHandlerTests
    {
        public StatusCodeHandlerTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddCarter(); })
                .Configure(x => x.UseCarter())
            );
            this.httpClient = this.server.CreateClient();
        }

        private readonly TestServer server;

        private readonly HttpClient httpClient;

        [Fact]
        public async Task Should_use_status_code_handlers()
        {
            var response = await this.httpClient.GetAsync("/statushandler");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World", body);
        }
    }

    public class StatusCodeHandlerModule : CarterModule
    {
        public StatusCodeHandlerModule()
        {
            this.Get("/statushandler", async (req, res, routeData) =>
            {
                res.StatusCode = 418;
                await res.WriteAsync("Hello");
            });
        }
    }

    public class TeapotStatusCodeHandler : IStatusCodeHandler
    {
        public bool CanHandle(int statusCode) => statusCode == 418;

        public async Task Handle(HttpContext ctx)
        {
            await ctx.Response.WriteAsync(" World");
        }
    }
}
