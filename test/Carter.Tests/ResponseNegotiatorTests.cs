namespace Carter.Tests
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;
    using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

    public class ResponseNegotiatorTests
    {
        public ResponseNegotiatorTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddCarter(); })
                .Configure(x => x.UseCarter())
            );
            this.httpClient = this.server.CreateClient();
        }

        private readonly TestServer server;

        private readonly HttpClient httpClient;

        [Theory]
        [InlineData("not/known")]
        [InlineData("utt$r-rubbish-9")]
        public async Task Should_fallback_to_json(string accept)
        {
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", accept);
            var response = await this.httpClient.GetAsync("/negotiate");
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Should_camelCase_json()
        {
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"firstName\":\"Jim\"}", body);
        }

        [Fact]
        public async Task Should_fallback_to_json_even_if_no_accept_header()
        {
            var response = await this.httpClient.GetAsync("/negotiate");
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Should_pick_correctly_weighted_processor()
        {
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.5));
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html", 0.3));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("XML Response", body);
        }

        [Fact]
        public async Task Should_pick_non_weighted_over_weighted()
        {
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.5));
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html", 0.3));
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("foo/bar"));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("FOOBAR", body);
        }

        [Fact]
        public async Task Should_use_appropriate_response_negotiator()
        {
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("foo/bar"));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("FOOBAR", body);
        }

        [Fact]
        public async Task Should_pick_default_json_processor_last()
        {
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.badger+json"));
            var response = await this.httpClient.GetAsync("/negotiate");
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("Non default json Response", body);
        }
    }

    internal class TestResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept) => accept.MediaType.ToString().IndexOf("foo/bar", StringComparison.OrdinalIgnoreCase) >= 0;

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken = default(CancellationToken))
        {
            await res.WriteAsync("FOOBAR", cancellationToken);
        }
    }

    internal class TestHtmlResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept) => accept.MediaType.ToString().IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken = default(CancellationToken))
        {
            await res.WriteAsync("HTML Response", cancellationToken);
        }
    }

    internal class TestXmlResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept) => accept.MediaType.ToString().IndexOf("application/xml", StringComparison.OrdinalIgnoreCase) >= 0;

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken = default(CancellationToken))
        {
            await res.WriteAsync("XML Response", cancellationToken);
        }
    }
    
    internal class TestJsonResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept) => accept.MediaType.ToString().IndexOf("application/vnd.badger+json", StringComparison.OrdinalIgnoreCase) >= 0;

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken = default(CancellationToken))
        {
            await res.WriteAsync("Non default json Response", cancellationToken);
        }
    }
}
