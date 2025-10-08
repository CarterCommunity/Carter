namespace Carter.Tests.ContentNegotiation
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Xunit;

    public class HttpJsonOptionsResponseNegotiatorTests
    {
        private async Task SetupServer()
        {
            var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer() // If using TestServer
                        .ConfigureServices(x =>
                        {
                            x.ConfigureHttpJsonOptions(jsonOptions =>
                            {
                                jsonOptions.SerializerOptions.PropertyNamingPolicy =
                                    JsonNamingPolicy.KebabCaseUpper;
                            });

                            x.AddRouting();
                            x.AddCarter(configurator: c => { c.WithModule<NegotiatorModule>(); });
                        })
                        .Configure(x =>
                        {
                            x.UseRouting();
                            x.UseEndpoints(builder => builder.MapCarter());
                        })
                        //.UseKestrel()
                        ;
                })
                .Build();

            await host.StartAsync();

            this.httpClient = host.GetTestClient();
        }

        private HttpClient httpClient;

        [Fact]
        public async Task Should_obey_httpjsonoptions()
        {
            await this.SetupServer();
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"FIRST-NAME\":\"Jim\"}", body);
        }
    }
}
