namespace Botwin.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class BotwinModuleTests
    {
        private readonly TestServer server;

        private readonly HttpClient httpClient;

        public BotwinModuleTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x =>
                {
                    x.AddSingleton<IAssemblyProvider, TestAssemblyProvider>();
                    x.AddBotwin();
                })
                .Configure(x => x.UseBotwin())
            );
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_return_GET_requests()
        {
            var response = await this.httpClient.GetAsync("/");

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_POST_requests()
        {
            var response = await this.httpClient.PostAsync("/", new StringContent(""));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_PUT_requests()
        {
            var response = await this.httpClient.PutAsync("/", new StringContent(""));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_DELETE_requests()
        {
            var response = await this.httpClient.DeleteAsync("/");

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_HEAD_requests()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/head"));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_HEAD_requests_for_defined_GET_routes()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"));

            Assert.Equal(200, (int)response.StatusCode);
        }
        
        [Fact]
        public async Task Should_return_OPTIONS_requests()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/"));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_PATCH_requests()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"),"/"));
            Assert.Equal(200, (int)response.StatusCode);
        }


        [Fact]
        public async Task Should_handle_module_before_hook()
        {
            var response = await this.httpClient.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(body.Contains("Before"));
        }

        [Fact]
        public async Task Should_handle_short_circuit_response_in_before_hook()
        {
            var response = await this.httpClient.GetAsync("/noaccess");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("NoAccessBefore", body);
        }

        [Fact]
        public async Task Should_handle_module_after_hook()
        {
            var response = await this.httpClient.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();
            Assert.True(body.Contains("After"));
        }
    }
}
