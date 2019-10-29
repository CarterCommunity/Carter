``` ini

BenchmarkDotNet=v0.11.5, OS=manjaro 
Intel Xeon E-2176M CPU 2.70GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100-preview1-014459
  [Host]     : .NET Core 2.2.7 (CoreCLR 4.6.28008.02, CoreFX 4.6.28008.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.7 (CoreCLR 4.6.28008.02, CoreFX 4.6.28008.03), 64bit RyuJIT


```
|    Method |     Mean |     Error |    StdDev |
|---------- |---------:|----------:|----------:|
| AddCarter | 2.910 ms | 0.0350 ms | 0.0327 ms |
