namespace Carter.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ModuleLifetimeTests
    {
        private TestServer server;

        private HttpClient httpClient;

        private void ConfigureServer()
        {
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(x =>
                {
                    x.AddScoped<ScopedRequestDependency>();
                    x.AddTransient<TransientRequestDependency>();
                    x.AddCarter();
                })
                .Configure(x => x.UseCarter()));
            
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
            var first = await this.httpClient.GetStringAsync("/scopedreqdep");
            var second = await this.httpClient.GetStringAsync("/scopedreqdep");

            Assert.NotEqual(first, second);
        }
        
         
        [Fact]
        public async Task Should_resolve_new_transient_dependency_per_request()
        {
            this.ConfigureServer();
            var first = await this.httpClient.GetStringAsync("/transientreqdep");
            var second = await this.httpClient.GetStringAsync("/transientreqdep");

            Assert.NotEqual(first, second);
        }
    }

    public class ScopedRequestDependency
    {
        private string instanceId;

        public ScopedRequestDependency()
        {
            this.instanceId = Guid.NewGuid().ToString();
        }

        public string GetGuid()
        {
            return this.instanceId;
        }
    }
    
    public class TransientRequestDependency
    {
        private readonly string instanceId;

        public TransientRequestDependency()
        {
            this.instanceId = Guid.NewGuid().ToString();
        }

        public string GetGuid()
        {
            return instanceId;
        }
    }

    public class ScopedRequestDependencyModule : CarterModule
    {
        public ScopedRequestDependencyModule(ScopedRequestDependency scopedRequestDependency)
        {
            this.Get("/scopedreqdep", ctx=>ctx.Response.WriteAsync(scopedRequestDependency.GetGuid()));
        }
    }
    
    public class TransientRequestDependencyModule : CarterModule
    {
        public TransientRequestDependencyModule(ScopedRequestDependency scopedRequestDependency)
        {
            this.Get("/transientreqdep", ctx=>ctx.Response.WriteAsync(scopedRequestDependency.GetGuid()));
        }
    }
}
