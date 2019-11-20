namespace Carter.Tests.StatusCodeHandlers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class StatusCodeHandlerTests
    {
        public StatusCodeHandlerTests()
        {
            this.server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddCarter(configurator: c =>
                            c.WithModule<StatusCodeHandlerModule>()
                             .WithStatusCodeHandler<TeapotStatusCodeHandler>());
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    })
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
}
