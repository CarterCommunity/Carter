namespace Carter.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Microsoft.Extensions.DependencyInjection;

    [InProcess, MemoryDiagnoser]
    public class AddCarterBenchmarks
    {
        private ServiceCollection services;

        [GlobalSetup]
        public void Setup()
        {
            this.services = new ServiceCollection();
        }

        [Benchmark]
        public void AddCarter() => this.services.AddCarter();
    }
}
