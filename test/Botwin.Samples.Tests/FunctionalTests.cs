using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore;

namespace Botwin.Samples.Tests
{
    public class FunctionalTests
    {
        private TestServer server;
        private HttpClient client;

        public FunctionalTests()
        {
            server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
            );

            client = server.CreateClient();
        }

        [Fact]
        public async Task Should_return_actor_data()
        {
            RouteHandlers.ListDirectorsHandler = () => GetDirectorsRoute.Handle(() => new[] { new Director { Name = "Ridley Scott" } }, () => true);


            var res = await client.GetAsync("/functional/directors");

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.True((await res.Content.ReadAsStringAsync()).Contains("Ridley"));
        }

        [Fact]
        public async Task Should_return_null_if_permission_not_allowed()
        {
            RouteHandlers.ListDirectorsHandler = () => GetDirectorsRoute.Handle(() => Enumerable.Empty<Director>(), () => false);

            var res = await client.GetAsync("/functional/directors");

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}