namespace Carter.Tests.ContentNegotiation
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class HttpJsonOptionsResponseNegotiatorTests
    {
        public HttpJsonOptionsResponseNegotiatorTests()
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.ConfigureHttpJsonOptions(jsonOptions =>
                        {
                            jsonOptions.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseUpper;
                        });
                        x.AddRouting();
                        x.AddCarter(configurator: c =>
                            c.WithModule<NegotiatorModule>());
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    })
            );
            this.httpClient = server.CreateClient();
        }

        private readonly HttpClient httpClient;

        [Fact]
        public async Task Should_obey_httpjsonoptions()
        {
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"FIRST-NAME\":\"Jim\"}", body);
        }
    }
}