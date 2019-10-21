namespace Carter.Benchmarks.StartupBenchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class StartupTests
    {
        [Fact]
        public void StartupCsTest()
        {
            var result = BenchmarkRunner.Run<Startup>();
            Assert.All(result.Reports, report => Assert.True(report.Success));
        }
    }

    [InProcessAttribute]
    public class Startup
    {
        [Benchmark]
        public void EntryPoint()
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter();
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.UseCarter();
        }
    }

    public class TestModule : CarterModule
    {
        public TestModule()
        {
            Get("/", async (req, res, routeData) => await res.WriteAsync("Hello from Carter!"));
        }
    }
}
