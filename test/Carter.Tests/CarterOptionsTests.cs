namespace Carter.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class CarterOptionsTests
    {
        private TestServer server;

        private HttpClient httpClient;

        private void ConfigureServer(bool continueRequest = true)
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddCarter(); })
                .Configure(x => x.UseCarter(new CarterOptions(async ctx =>
                {
                    await ctx.Response.WriteAsync("GlobalBefore");
                    return continueRequest;
                }, async ctx => await ctx.Response.WriteAsync("GlobalAfter"))))
            );
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_apply_global_after_hook()
        {
            this.ConfigureServer();
            var response = await this.httpClient.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(body.Contains("GlobalAfter"));
        }

        [Fact]
        public async Task Should_apply_global_before_hook()
        {
            this.ConfigureServer();
            var response = await this.httpClient.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(body.Contains("GlobalBefore"));
        }

        [Fact]
        public async Task Should_short_circuit_global_before_hook()
        {
            this.ConfigureServer(false);
            var response = await this.httpClient.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("GlobalBefore", body);
        }
    }
}
