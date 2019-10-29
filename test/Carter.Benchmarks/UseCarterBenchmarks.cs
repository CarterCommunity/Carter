namespace Carter.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Microsoft.AspNetCore.Builder.Internal;
    using Microsoft.Extensions.DependencyInjection;

    [InProcess, MemoryDiagnoser]
    public class UseCarterBenchmarks
    {
        private ApplicationBuilder app;
        
        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddCarter();
            this.app = new ApplicationBuilder(serviceCollection.BuildServiceProvider());
        }

        [Benchmark]
        public void UseCarter() => this.app.UseCarter();
    }
}
