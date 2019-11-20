namespace Carter.Tests.DependencyInjection
{
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ModuleLifetimeTests
    {
        /*
         
         * Transient objects are always different; a new instance is provided to every controller and every service.

         * Scoped objects are the same within a request, but different across different requests.

         * Singleton objects are the same for every object and every request.
         
        */
        private TestServer server;

        private HttpClient httpClient;

        private void ConfigureServer()
        {
            this.server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(x =>
                    {
                        x.AddScoped<ScopedRequestDependency>();
                        x.AddScoped<ScopedServiceDependency>();
                        x.AddTransient<TransientRequestDependency>();
                        x.AddTransient<TransientServiceDependency>();
                        x.AddCarter(configurator: c =>
                            c.WithModule<ScopedRequestDependencyModule>()
                                .WithModule<TransientRequestDependencyModule>());
                    })
                    .Configure(x =>
                    {
                        x.UseRouting();
                        x.UseEndpoints(builder => builder.MapCarter());
                    }));
            
            this.httpClient = this.server.CreateClient();
        }

        [Fact]
        public async Task Should_resolve_new_module_foreach_request()
        {
            this.ConfigureServer();
            var first = await this.httpClient.GetStringAsync("/instanceid");
            var second = await this.httpClient.GetStringAsync("/instanceid");

            Assert.NotEqual(first, second);
        }

        [Fact]
        public async Task Should_resolve_new_scoped_dependency_per_request()
        {
            this.ConfigureServer();

            //Scoped uses the same instance per request so each value separated by a : should be the same
            var first = await this.httpClient.GetStringAsync("/scopedreqdep");
            Assert.Single(first.Split(":").Distinct());

            var second = await this.httpClient.GetStringAsync("/scopedreqdep");
            Assert.Single(second.Split(":").Distinct());

            Assert.NotEqual(first, second);
        }

        [Fact]
        public async Task Should_resolve_new_transient_dependency_per_request()
        {
            this.ConfigureServer();

            //Transient uses a different instance in each object per request so each value separated by a : should be different
            var first = await this.httpClient.GetStringAsync("/transientreqdep");
            Assert.Equal(2, first.Split(":").Distinct().Count());

            var second = await this.httpClient.GetStringAsync("/transientreqdep");
            Assert.Equal(2, second.Split(":").Distinct().Count());

            Assert.NotEqual(first, second);
        }
    }
}
