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

    [InProcess, MemoryDiagnoser]
    public class Startup
    {
        [Benchmark]
        public void Configure()
        {
            new WebHostBuilder()
                .Configure(app => app.UseCarter())
                .Build();
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
