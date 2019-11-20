namespace Carter.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;

    public class SameRouteTests
    {
        public SameRouteTests()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x => { x.AddCarter(configurator: configurator => configurator.WithModule<SameRouteModule>()); })
                .Configure(x =>
                {
                    x.UseRouting();
                    x.UseEndpoints(builder => builder.MapCarter());
                }));

            this.httpClient = this.server.CreateClient();
        }

        private readonly TestServer server;

        private readonly HttpClient httpClient;

        [Fact]
        public async Task Should_get_response_from_route_with_exact_same_route_constraints_but_different_names()
        {
            var response = await this.httpClient.GetAsync("/sametest/42/blah/d9796528-4631-44e2-b898-6caf2706467d");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
