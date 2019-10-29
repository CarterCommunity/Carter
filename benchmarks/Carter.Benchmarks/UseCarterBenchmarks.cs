namespace Carter.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Microsoft.AspNetCore.Builder.Internal;
    using Microsoft.Extensions.DependencyInjection;

    public class UseCarterBenchmarks
    {
        private ApplicationBuilder app;
        
        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddCarter();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            this.app = new ApplicationBuilder(serviceProvider);
        }

        [Benchmark]
        public void UseCarter() => this.app.UseCarter();
    }
}
