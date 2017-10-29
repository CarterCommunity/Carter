namespace Botwin.Tests
{
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class StatusCodeHandlerTests
    {
        private readonly TestServer server;
        private readonly HttpClient httpClient;

        public StatusCodeHandlerTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                                       .ConfigureServices(x =>
                                       {
                                           x.AddBotwin(typeof(TestModule).GetTypeInfo().Assembly);
                                       })
                                       .Configure(x => x.UseBotwin())
                                   );
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_use_status_code_handlers()
        {
            var response = await this.httpClient.GetAsync("/statushandler");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello World", body);
        }
    }

    public class StatusCodeHandlerModule : BotwinModule
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
        public bool CanHandle(int statusCode)
        {
            return statusCode == 418;
        }

        public async Task Handle(HttpContext ctx)
        {
            await ctx.Response.WriteAsync(" World");
        }
    }
}