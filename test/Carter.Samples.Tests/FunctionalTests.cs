namespace Carter.Samples.Tests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using CarterSample;
    using CarterSample.Features.FunctionalProgramming;
    using CarterSample.Features.FunctionalProgramming.CreateDirector;
    using CarterSample.Features.FunctionalProgramming.DeleteDirector;
    using CarterSample.Features.FunctionalProgramming.GetDirectorById;
    using CarterSample.Features.FunctionalProgramming.ListDirectors;
    using CarterSample.Features.FunctionalProgramming.UpdateDirector;
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
            this.server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
            );

            this.client = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_return_list_of_director_data()
        {
            RouteHandlers.ListDirectorsHandler = () => ListDirectorsRoute.Handle(() => new[] { new Director { Name = "Ridley Scott" } }, () => true, () => false);

            var res = await this.client.GetAsync("/functional/directors");

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("Ridley", await res.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Should_return_null_if_permission_not_allowed()
        {
            RouteHandlers.ListDirectorsHandler = () => ListDirectorsRoute.Handle(() => Enumerable.Empty<Director>(), () => false, () => false);

            RouteHandlers.UpdateDirectorHandler = director => UpdateDirectorRoute.Handle(director, director1 => 9, () => true);

            var res = await this.client.GetAsync("/functional/directors");

            Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
        }

        [Fact]
        public async Task Should_return_director_by_id()
        {
            RouteHandlers.GetDirectorByIdHandler = dirId => GetDirectorByIdRoute.Handle(dirId, id => new Director { Name = id.ToString() }, () => true);

            var res = await this.client.GetAsync("/functional/directors/123");

            Assert.Contains("123", await res.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Should_store_director()
        {
            //Given
            RouteHandlers.CreateDirectorHandler = director => CreateDirectorRoute.Handle(director, newDirector => 1);

            //When
            var res = await this.client.PostAsync("/functional/directors", new StringContent(JsonConvert.SerializeObject(new Director { Name = "Jon Favreau" }), Encoding.UTF8, "application/json"));

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

        [Fact]
        public async Task Should_return_422_updating_dodgy_director()
        {
            //Given
            RouteHandlers.UpdateDirectorHandler = director => UpdateDirectorRoute.Handle(director, newDirector => 1, () => true);

            //When
            var res = await this.client.PutAsync("/functional/directors/1", new StringContent(JsonConvert.SerializeObject(new Director { Name = "" }), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(422, (int)res.StatusCode);
        }

        [Fact]
        public async Task Should_update_director()
        {
            //Given
            RouteHandlers.UpdateDirectorHandler = director => UpdateDirectorRoute.Handle(director, newDirector => 1, () => true);

            //When
            var res = await this.client.PutAsync("/functional/directors/1", new StringContent(JsonConvert.SerializeObject(new Director { Name = "Plop" }), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(204, (int)res.StatusCode);
        }

        [Fact]
        public async Task Should_return_403_if_user_not_allowed_on_update()
        {
            //Given
            RouteHandlers.UpdateDirectorHandler = director => UpdateDirectorRoute.Handle(director, newDirector => 1, () => false);

            //When
            var res = await this.client.PutAsync("/functional/directors/1", new StringContent(JsonConvert.SerializeObject(new Director { Name = "Plop" }), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(403, (int)res.StatusCode);
        }

        [Fact]
        public async Task Should_return_400_if_user_not_successfully_updated()
        {
            //Given
            RouteHandlers.UpdateDirectorHandler = director => UpdateDirectorRoute.Handle(director, newDirector => 0, () => true);

            //When
            var res = await this.client.PutAsync("/functional/directors/1", new StringContent(JsonConvert.SerializeObject(new Director { Name = "Plop" }), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(400, (int)res.StatusCode);
        }

        [Fact]
        public async Task Should_delete_director()
        {
            //Given
            RouteHandlers.DeleteDirectorHandler = id => DeleteDirectorRoute.Handle(id, newDirector => 1, () => true);

            //When
            var res = await this.client.DeleteAsync("/functional/directors/1");

            //Then
            Assert.Equal(204, (int)res.StatusCode);
        }

        [Fact]
        public async Task Should_return_403_if_user_not_allowed_on_delete()
        {
            //Given
            RouteHandlers.DeleteDirectorHandler = id => DeleteDirectorRoute.Handle(id, newDirector => 1, () => false);

            //When
            var res = await this.client.DeleteAsync("/functional/directors/1");

            //Then
            Assert.Equal(403, (int)res.StatusCode);
        }
    }
}
