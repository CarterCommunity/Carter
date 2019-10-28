namespace Carter.Benchmarks.StartupBenchmarks
{
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Builder.Internal;
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
        private ApplicationBuilder applicationBuilder;

        public IEnumerable<ServiceCollection> ServiceCollection()
        {
            yield return new ServiceCollection();
        }
        
        public IEnumerable<IApplicationBuilder> ApplicationBuilder()
        {
            yield return applicationBuilder;
        }
        
        [IterationSetup]
        public void IterationSetup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddCarter();
            this.applicationBuilder = new ApplicationBuilder(serviceCollection.BuildServiceProvider());
        }

        [Benchmark]
        [ArgumentsSource(nameof(ServiceCollection))]
        public void ConfigureServices(IServiceCollection services) => services.AddCarter();

        [Benchmark]
        [ArgumentsSource(nameof(ApplicationBuilder))]
        public void Configure(IApplicationBuilder app) => app.UseCarter();
    }
}
