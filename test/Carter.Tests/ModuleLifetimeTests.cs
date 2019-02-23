namespace Carter.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class ModuleLifetimeTests
    {
        private TestServer server;

        private HttpClient httpClient;

        private void ConfigureServer(bool continueRequest = true)
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddCarter(); })
                .Configure(x => x.UseCarter()));
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_resolve_new_module_foreach_request()
        {
            this.ConfigureServer();
            var first = await this.httpClient.GetStringAsync("/instanceid");
            var second = await this.httpClient.GetStringAsync("/instanceid");

            Assert.NotEqual(first, second);
        }
    }
}
