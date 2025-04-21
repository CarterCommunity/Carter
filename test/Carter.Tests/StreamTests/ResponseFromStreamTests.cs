namespace Carter.Tests.StreamTests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;

    public class ResponseFromStreamTests
    {
        public ResponseFromStreamTests()
        {
            this.server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddRouting();
                        x.AddCarter(configurator: c => c.WithModule<StreamModule>());
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    }));
            this.httpClient = this.server.CreateClient();
        }

        private readonly TestServer server;

        private readonly HttpClient httpClient;

        [Test]
        [Arguments("0-2", "0-2", "012")]
        [Arguments("2-4", "2-4", "234")]
        [Arguments("4-6", "4-6", "456")]
        [Arguments("0-", "0-9", "0123456789")]
        public async Task Should_return_range(string range, string expectedRange, string expectedBody)
        {
            //Given & When
            this.httpClient.DefaultRequestHeaders.Range = RangeHeaderValue.Parse($"bytes={range}");
            var response = await this.httpClient.GetAsync("/downloadrange");

            var body = await response.Content.ReadAsStringAsync();

            //Then
            await Assert.That( body).IsEqualTo(expectedBody);
            await Assert.That( response.StatusCode).IsEqualTo(HttpStatusCode.PartialContent);
            await Assert.That( response.Content.Headers.ContentRange.ToString()).IsEqualTo($"bytes {expectedRange}/10");
        }

        [Test]
        [Arguments("-1")]
        [Arguments("0-9999999999")]
        public async Task Should_return_requested_range_not_satisfiable(string range)
        {
            //Given & When
            this.httpClient.DefaultRequestHeaders.Range = RangeHeaderValue.Parse($"bytes={range}");
            var response = await this.httpClient.GetAsync("/downloadrange");

            //Then
            await Assert.That( response.StatusCode).IsEqualTo(HttpStatusCode.RequestedRangeNotSatisfiable);
        }

        [Test]
        [Arguments("3-1")]
        [Arguments("1+2")]
        public async Task Should_return_full_stream_on_invalid_headers(string range)
        {
            //Given
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Range", $"bytes={range}");

            //When
            var response = await this.httpClient.GetAsync("/downloadrange");

            var body = await response.Content.ReadAsStringAsync();

            //Then
            await Assert.That( response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            await Assert.That( body).IsEqualTo("0123456789");
        }

        [Test]
        public async Task Should_not_set_content_disposition_header_by_default()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/download");
            var body = await response.Content.ReadAsStringAsync();

            //Then
            await Assert.That(response.Content.Headers.ContentDisposition).IsNull();
            await Assert.That( response.Content.Headers.ContentType.MediaType).IsEqualTo("application/csv");
            await Assert.That( body).IsEqualTo("hi");
            await Assert.That( response.Headers.AcceptRanges.FirstOrDefault()).IsEqualTo("bytes");
        }

        [Test]
        public async Task Should_set_content_type_body_acceptrange_header_content_disposition()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/downloadwithcd");
            var body = await response.Content.ReadAsStringAsync();
            var filename = response.Content.Headers.ContentDisposition.FileName;

            //Then
            await Assert.That( response.Content.Headers.ContentType.MediaType).IsEqualTo("application/csv");
            await Assert.That( body).IsEqualTo("hi");
            await Assert.That( response.Headers.AcceptRanges.FirstOrDefault()).IsEqualTo("bytes");
            await Assert.That( filename).IsEqualTo("journal.csv");
        }
    }
}
