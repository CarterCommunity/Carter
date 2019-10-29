namespace Carter.Benchmarks
{
    using BenchmarkDotNet.Running;
    using Xunit;

    public class Harness
    {
        [Fact]
        public void AddCarterBenchmark()
        {
            BenchmarkRunner.Run<AddCarterBenchmarks>();
        }
        
        [Fact]
        public void UseCarterBenchmark()
        {
            BenchmarkRunner.Run<UseCarterBenchmarks>();
        }
    }
}
