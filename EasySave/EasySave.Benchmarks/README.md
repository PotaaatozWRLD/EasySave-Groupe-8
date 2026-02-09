# EasySave Performance Benchmarks

This project contains performance benchmarks for EasySave backup operations using [BenchmarkDotNet](https://benchmarkdotnet.org/).

## 🎯 Purpose

Track performance metrics across versions to:

- Identify performance regressions
- Optimize file transfer operations
- Compare Full vs Differential backup speeds
- Measure memory allocation and throughput

## 🚀 Running Benchmarks

### Prerequisites

- .NET 10.0 SDK or later
- **Release mode** (required by BenchmarkDotNet)

### Command Line

```bash
cd EasySave/EasySave.Benchmarks
dotnet run -c Release
```

### From Visual Studio

1. Set configuration to **Release** (not Debug)
2. Set `EasySave.Benchmarks` as startup project
3. Press F5 or Ctrl+F5

## 📊 Benchmark Scenarios

| Benchmark | Description | File Count | File Size |
|-----------|-------------|------------|-----------|
| **Copy_10_SmallFiles_1KB** | Copy small files | 10 | 1 KB each |
| **Copy_100_MediumFiles_1MB** | Copy medium files | 100 | 1 MB each |
| **Copy_1000_SmallFiles_1KB** | Copy many small files | 1000 | 1 KB each |
| **Copy_10_LargeFiles_10MB** | Copy large files | 10 | 10 MB each |
| **CalculateDirectorySize_100Files** | Calculate total size | 100 | 1 MB each |
| **CountFiles_1000Files** | Count files recursively | 1000 | 1 KB each |

## 📈 Sample Output

```
BenchmarkDotNet=v0.15.8, OS=Windows 11
Intel Core i7-9700K CPU 3.60GHz, 1 CPU, 8 logical and 8 physical cores
.NET SDK=10.0.102
  [Host]     : .NET 10.0.2 (10.0.226.6906), X64 RyuJIT

|                           Method |      Mean |    Error |   StdDev |    Median | Allocated |
|--------------------------------- |----------:|---------:|---------:|----------:|----------:|
|       Copy_10_SmallFiles_1KB     |  12.34 ms | 0.245 ms | 0.789 ms |  12.18 ms |  45.2 KB  |
|     Copy_100_MediumFiles_1MB     | 234.56 ms | 4.567 ms | 8.901 ms | 230.12 ms | 123.4 KB  |
|    Copy_1000_SmallFiles_1KB      | 156.78 ms | 3.123 ms | 6.789 ms | 154.23 ms | 234.5 KB  |
|      Copy_10_LargeFiles_10MB     | 567.89 ms | 8.901 ms | 12.34 ms | 563.45 ms |  78.9 KB  |
| CalculateDirectorySize_100Files  |  89.01 ms | 1.234 ms | 3.456 ms |  88.23 ms |  12.3 KB  |
|         CountFiles_1000Files     |  23.45 ms | 0.456 ms | 1.234 ms |  23.12 ms |   8.9 KB  |
```

## 📂 Results Location

BenchmarkDotNet generates detailed reports in:

```
EasySave.Benchmarks/BenchmarkDotNet.Artifacts/results/
```

Formats available:

- **HTML** - Interactive charts and tables
- **CSV** - Raw data for analysis
- **Markdown** - GitHub-friendly format

## 🔧 Customizing Benchmarks

### Add New Benchmark

```csharp
[Benchmark]
public void YourNewBenchmark()
{
    // Your benchmark code here
}
```

### Change Iterations

```csharp
[SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 15)]
public class BackupBenchmarks
{
    // ...
}
```

### Add Memory Profiling

```csharp
[MemoryDiagnoser]  // Already enabled by default
[ThreadingDiagnoser]  // Add for thread analysis
public class BackupBenchmarks
{
    // ...
}
```

## 📝 Best Practices

1. **Always run in Release mode** - Debug builds are 10-100x slower
2. **Close other applications** - Reduces measurement noise
3. **Run multiple iterations** - BenchmarkDotNet does this automatically
4. **Benchmark relative changes** - Compare before/after results
5. **Use [GlobalSetup]** for expensive initialization
6. **Use [IterationSetup]** for per-benchmark setup

## 🎯 Performance Goals (v1.0)

| Operation | Target | Acceptable |
|-----------|--------|------------|
| 100 files (1 MB each) | <200 ms | <500 ms |
| 1000 small files (1 KB) | <150 ms | <300 ms |
| Directory size calc (100 files) | <100 ms | <200 ms |

## 📊 Tracking Performance Across Versions

1. Run benchmarks before code changes
2. Save results: `git add BenchmarkDotNet.Artifacts/results/`
3. Make your changes
4. Run benchmarks again
5. Compare results using BenchmarkDotNet's comparison tools

## 🔗 Resources

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html)
- [BenchmarkDotNet GitHub](https://github.com/dotnet/BenchmarkDotNet)
- [Performance Best Practices (.NET)](https://learn.microsoft.com/en-us/dotnet/framework/performance/)

---

**Note**: Benchmarks use temporary directories in `%TEMP%` and clean up automatically.
