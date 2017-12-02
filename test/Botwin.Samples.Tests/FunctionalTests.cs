using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Botwin.Samples;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
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
            Composition.FunctionalHandler = () => FunctionalRoute.Handle(() =>
                    {
                        return new[] { new Actor { Name = "John Travolta" } };
                    }, () => true);


            var res = await client.GetAsync("/functional");

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.True((await res.Content.ReadAsStringAsync()).Contains("Travolta"));
        }

        [Fact]
        public async Task Should_return_null_if_permission_not_allowed()
        {
            Composition.FunctionalHandler = () => FunctionalRoute.Handle(() =>
                    {
                        return Enumerable.Empty<Actor>();
                    }, () => false);


            var res = await client.GetAsync("/functional");

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}