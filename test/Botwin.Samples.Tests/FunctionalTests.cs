namespace Botwin.Samples.Tests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Botwin.Samples.CreateDirector;
    using Botwin.Samples.GetDirectorById;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

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
        public async Task Should_return_list_of_director_data()
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

        [Fact]
        public async Task Should_return_director_by_id()
        {
            RouteHandlers.GetDirectorByIdHandler = dirId => GetDirectorByIdRoute.Handle(dirId, id => new Director { Name = id.ToString() }, () => true);

            var res = await client.GetAsync("/functional/directors/123");

            Assert.True((await res.Content.ReadAsStringAsync()).Contains("123"));
        }

        [Fact]
        public async Task Should_store_director()
        {
            //Given
            RouteHandlers.CreateDirectorHandler = director => CreateDirectorRoute.Handle(director, newDirector => 1);

            //When
            var res = await this.client.PostAsync("/functional/directors", new StringContent(JsonConvert.SerializeObject(new Director() { Name = "Jon Favreau" }), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            Assert.NotNull(res.Headers.Location);
        }

        [Fact]
        public async Task Should_return_422_storing_dodgy_director()
        {
            //Given
            RouteHandlers.CreateDirectorHandler = director => CreateDirectorRoute.Handle(director, newDirector => 1);

            //When
            var res = await this.client.PostAsync("/functional/directors", new StringContent(JsonConvert.SerializeObject(new Director { Name = "" }), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(422, (int)res.StatusCode);
        }
    }
}
