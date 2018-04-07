namespace Carter.Tests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class ResponseFromStreamTests
    {
        public ResponseFromStreamTests()
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
        [InlineData("0-2", "0-2", "012")]
        [InlineData("2-4", "2-4", "234")]
        [InlineData("4-6", "4-6", "456")]
        [InlineData("0-", "0-9", "0123456789")]
        public async Task Should_return_range(string range, string expectedRange, string expectedBody)
        {
            //Given & When
            this.httpClient.DefaultRequestHeaders.Range = RangeHeaderValue.Parse($"bytes={range}");
            var response = await this.httpClient.GetAsync("/downloadrange");

            var body = await response.Content.ReadAsStringAsync();

            //Then
            Assert.Equal(expectedBody, body);
            Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
            Assert.Equal($"bytes {expectedRange}/10", response.Content.Headers.ContentRange.ToString());
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("0-9999999999")]
        public async Task Should_return_requested_range_not_satisfiable(string range)
        {
            //Given & When
            this.httpClient.DefaultRequestHeaders.Range = RangeHeaderValue.Parse($"bytes={range}");
            var response = await this.httpClient.GetAsync("/downloadrange");

            //Then
            Assert.Equal(HttpStatusCode.RequestedRangeNotSatisfiable, response.StatusCode);
        }

        [Theory]
        [InlineData("3-1")]
        [InlineData("1+2")]
        public async Task Should_return_full_stream_on_invalid_headers(string range)
        {
            //Given
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Range", $"bytes={range}");

            //When
            var response = await this.httpClient.GetAsync("/downloadrange");

            var body = await response.Content.ReadAsStringAsync();

            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("0123456789", body);
        }

        [Fact]
        public async Task Should_not_set_content_disposition_header_by_default()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/download");
            var body = await response.Content.ReadAsStringAsync();

            //Then
            Assert.Null(response.Content.Headers.ContentDisposition);
            Assert.Equal("application/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("hi", body);
            Assert.Equal("bytes", response.Headers.AcceptRanges.FirstOrDefault());
        }

        [Fact]
        public async Task Should_set_content_type_body_acceptrange_header_content_disposition()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/downloadwithcd");
            var body = await response.Content.ReadAsStringAsync();
            var filename = response.Content.Headers.ContentDisposition.FileName;

            //Then
            Assert.Equal("application/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("hi", body);
            Assert.Equal("bytes", response.Headers.AcceptRanges.FirstOrDefault());
            Assert.Equal("journal.csv", filename);
        }
    }
}
