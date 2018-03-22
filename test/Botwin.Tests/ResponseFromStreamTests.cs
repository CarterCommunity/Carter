namespace Botwin.Tests
{
    using System.Net.Http;
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
    }
}
