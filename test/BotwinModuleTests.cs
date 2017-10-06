namespace Botwin.Tests
{
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
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
                    x.AddBotwin(typeof(TestModule).GetTypeInfo().Assembly);
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
        public async Task Should_return_GET_requests_with_base_path()
        {
            var response = await this.httpClient.GetAsync("/test/");

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_POST_requests_with_base_path()
        {
            var response = await this.httpClient.PostAsync("/test/", new StringContent(""));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_PUT_requests_with_base_path()
        {
            var response = await this.httpClient.PutAsync("/test/", new StringContent(""));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_DELETE_requests_with_base_path()
        {
            var response = await this.httpClient.DeleteAsync("/test/");

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_HEAD_requests_with_base_path()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/test/head/"));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_HEAD_requests_for_defined_GET_routes_with_base_path()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/test/"));

            Assert.Equal(200, (int)response.StatusCode);
        }
        
        [Fact]
        public async Task Should_return_OPTIONS_requests_with_base_path()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/test/"));

            Assert.Equal(200, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_PATCH_requests_with_base_path()
        {
            var response = await this.httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"),"/test/"));
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
        
        [Fact]
        public async Task Should_return_GET_requests_with_parsed_querystring()
        {
            const int idToTest = 69; 
            var response = await this.httpClient.GetAsync($"/querystring?id={idToTest}");

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(200, (int)response.StatusCode);
            Assert.True(body.Contains($"Managed to parse an int {idToTest}"));
        }
        
        [Fact]
        public async Task Should_return_GET_requests_with_multiple_parsed_querystring()
        {
            var response = await this.httpClient.GetAsync($"/multiquerystring?id=1&id=2");

            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(200, (int)response.StatusCode);
            Assert.True(body.Contains($"Managed to parse multiple ints 2"));
        }
    }
}
