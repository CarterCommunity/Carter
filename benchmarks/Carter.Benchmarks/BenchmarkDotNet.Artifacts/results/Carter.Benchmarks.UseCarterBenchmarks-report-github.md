``` ini

BenchmarkDotNet=v0.12.0, OS=manjaro 
Intel Xeon E-2176M CPU 2.70GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.402
  [Host]     : .NET Core 2.2.7 (CoreCLR 4.6.28008.02, CoreFX 4.6.28008.03), X64 RyuJIT
  DefaultJob : .NET Core 2.2.7 (CoreCLR 4.6.28008.02, CoreFX 4.6.28008.03), X64 RyuJIT


```
|    Method |     Mean |    Error |   StdDev |
|---------- |---------:|---------:|---------:|
| UseCarter | 11.90 us | 0.213 us | 0.189 us |
