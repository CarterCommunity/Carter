namespace Botwin.Tests
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class ResponseFromStreamTests
    {
        private TestServer server;

        private HttpClient httpClient;

        public ResponseFromStreamTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddBotwin(); })
                .Configure(x => x.UseBotwin())
            );
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_set_content_type()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/downloadwithcd");

            //Then
            Assert.Equal("application/csv", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task Should_set_accept_ranges()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/downloadwithcd");

            //Then
            Assert.Equal("bytes", response.Headers.AcceptRanges.FirstOrDefault());
        }

        [Fact]
        public async Task Should_copy_stream_to_body()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/downloadwithcd");

            var body = await response.Content.ReadAsStringAsync();

            //Then
            Assert.Equal("hi", body);
        }

        [Fact]
        public async Task Should_set_content_disposition_header_if_supplied()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/downloadwithcd");

            var filename = response.Content.Headers.ContentDisposition.FileName;

            //Then
            Assert.Equal("journal.csv", filename);
        }

        [Fact]
        public async Task Should_not_set_content_disposition_header_by_default()
        {
            //Given & When
            var response = await this.httpClient.GetAsync("/download");

            //Then
            Assert.Null(response.Content.Headers.ContentDisposition);
        }

        [Theory]
        [InlineData("0-2", "012")]
        [InlineData("2-4", "234")]
        [InlineData("4-6", "456")]
        public async Task Should_return_range(string range, string expectedBody)
        {
            //Given & When
            this.httpClient.DefaultRequestHeaders.Range = RangeHeaderValue.Parse($"bytes={range}");
            var response = await this.httpClient.GetAsync("/downloadrange");

            var body = await response.Content.ReadAsStringAsync();
            //Then
            Assert.Equal(expectedBody, body);
            Assert.Equal($"bytes {range}/10", response.Content.Headers.ContentRange.ToString());
        }
    }
}
