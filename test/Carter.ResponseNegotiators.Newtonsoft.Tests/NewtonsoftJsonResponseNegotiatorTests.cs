namespace Carter.ResponseNegotiators.Newtonsoft.Tests
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Xunit;
    using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

    public class NewtonsoftJsonResponseNegotiatorTests
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
                            x.AddRouting();
                            x.AddCarter(configurator: c =>
                            {
                                c.WithModule<NewtonsoftJsonResponseNegotiatorModule>();
                                c.WithResponseNegotiator<TestJsonResponseNegotiator>();
                                c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>();
                            });
                        })
                        .Configure(x =>
                        {
                            x.UseRouting();
                            x.UseEndpoints(builder => builder.MapCarter());
                        });
                })
                .Build();

            await host.StartAsync();

            this.httpClient = host.GetTestClient();
        }

        private HttpClient httpClient;

        [Theory]
        [InlineData("not/known")]
        [InlineData("utt$r-rubbish-9")]
        public async Task Should_fallback_to_json(string accept)
        {
            await this.SetupServer();
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", accept);
            var response = await this.httpClient.GetAsync("/negotiate");
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Should_camelCase_json()
        {
            await this.SetupServer();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"firstName\":\"Jim\"}", body);
        }

        [Fact]
        public async Task Should_fallback_to_json_even_if_no_accept_header()
        {
            await this.SetupServer();
            var response = await this.httpClient.GetAsync("/negotiate");
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Should_pick_default_json_processor_last()
        {
            await this.SetupServer();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.badger+json"));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("Non default json Response", body);
        }
    }

    //TODO: Add [TestNegotiator] attribute when Carter supports it in this project
    internal class TestJsonResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept) => accept
            .MediaType.ToString()
            .IndexOf("application/vnd.badger+json",
                StringComparison.OrdinalIgnoreCase) >= 0;

        public async Task Handle<T>(HttpRequest req, HttpResponse res, T model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await res.WriteAsync("Non default json Response", cancellationToken);
        }
    }
}
