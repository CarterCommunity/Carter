namespace Carter.Samples.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using CarterSample;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.TestHost;
    using Shouldly;
    using Shouldly.Configuration;
    using Xunit;

    public class OpenAPITests
    {
        private HttpClient client;

        public OpenAPITests()
        {
          

            var server = new TestServer(WebHost.CreateDefaultBuilder()
                    .UseStartup<Startup>()
            );

            this.client = server.CreateClient();
        }

        [Fact]
        public async Task Should_return_Carter_approved_OpenAPI_json()
        {
            ShouldlyConfiguration.DiffTools.RegisterDiffTool(new DiffTool("diffmerge",
                new DiffToolConfig
                {
                    LinuxPath = "/usr/local/bin",
                    MacPath = "/usr/local/bin/diffmerge",
                    WindowsPath = "/usr/local/bin"
                },
                (received, approved, exists) => { return approved; }));

            var res = await this.client.GetAsync("/openapi");

            var content = await res.Content.ReadAsStringAsync();

            content.ShouldMatchApproved();
        }
    }
}
