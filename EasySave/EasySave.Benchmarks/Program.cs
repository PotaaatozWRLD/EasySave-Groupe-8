using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.IO;

namespace EasySave.Benchmarks;

/// <summary>
/// Entry point for running benchmarks.
/// Run with: dotnet run -c Release
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<BackupBenchmarks>();
    }
}

/// <summary>
/// Performance benchmarks for EasySave backup operations.
/// Measures throughput, memory allocation, and execution time.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class BackupBenchmarks
{
    private string _sourcePath = null!;
    private string _targetPath = null!;
    private const int SmallFileCount = 10;
    private const int MediumFileCount = 100;
    private const int LargeFileCount = 1000;
    private const int SmallFileSize = 1024; // 1 KB
    private const int MediumFileSize = 1024 * 1024; // 1 MB
    private const int LargeFileSize = 10 * 1024 * 1024; // 10 MB

    [GlobalSetup]
    public void Setup()
    {
        // Create temporary directories for benchmarks
        _sourcePath = Path.Combine(Path.GetTempPath(), "EasySave_Bench_Source");
        _targetPath = Path.Combine(Path.GetTempPath(), "EasySave_Bench_Target");

        Directory.CreateDirectory(_sourcePath);
        Directory.CreateDirectory(_targetPath);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        // Clean up temporary directories
        if (Directory.Exists(_sourcePath))
            Directory.Delete(_sourcePath, true);
        if (Directory.Exists(_targetPath))
            Directory.Delete(_targetPath, true);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // Clean target directory before each benchmark iteration
        if (Directory.Exists(_targetPath))
            Directory.Delete(_targetPath, true);
        Directory.CreateDirectory(_targetPath);
    }

    /// <summary>
    /// Benchmark: Copy 10 small files (1 KB each)
    /// </summary>
    [Benchmark]
    public void Copy_10_SmallFiles_1KB()
    {
        CreateTestFiles(_sourcePath, SmallFileCount, SmallFileSize);
        CopyDirectory(_sourcePath, _targetPath);
    }

    /// <summary>
    /// Benchmark: Copy 100 medium files (1 MB each)
    /// </summary>
    [Benchmark]
    public void Copy_100_MediumFiles_1MB()
    {
        CreateTestFiles(_sourcePath, MediumFileCount, MediumFileSize);
        CopyDirectory(_sourcePath, _targetPath);
    }

    /// <summary>
    /// Benchmark: Copy 1000 small files (1 KB each)
    /// </summary>
    [Benchmark]
    public void Copy_1000_SmallFiles_1KB()
    {
        CreateTestFiles(_sourcePath, LargeFileCount, SmallFileSize);
        CopyDirectory(_sourcePath, _targetPath);
    }

    /// <summary>
    /// Benchmark: Copy 10 large files (10 MB each)
    /// </summary>
    [Benchmark]
    public void Copy_10_LargeFiles_10MB()
    {
        CreateTestFiles(_sourcePath, SmallFileCount, LargeFileSize);
        CopyDirectory(_sourcePath, _targetPath);
    }

    /// <summary>
    /// Benchmark: Calculate total size of 100 files
    /// </summary>
    [Benchmark]
    public long CalculateDirectorySize_100Files()
    {
        CreateTestFiles(_sourcePath, MediumFileCount, MediumFileSize);
        return CalculateTotalSize(_sourcePath);
    }

    /// <summary>
    /// Benchmark: Count files recursively in directory with 1000 files
    /// </summary>
    [Benchmark]
    public int CountFiles_1000Files()
    {
        CreateTestFiles(_sourcePath, LargeFileCount, SmallFileSize);
        return CountFilesRecursive(_sourcePath);
    }

    #region Helper Methods

    private void CreateTestFiles(string directory, int count, int fileSizeBytes)
    {
        // Clear directory first
        foreach (var file in Directory.GetFiles(directory))
            File.Delete(file);

        // Create test files
        var buffer = new byte[fileSizeBytes];
        new Random(42).NextBytes(buffer); // Deterministic random data

        for (int i = 0; i < count; i++)
        {
            string filePath = Path.Combine(directory, $"testfile_{i}.dat");
            File.WriteAllBytes(filePath, buffer);
        }
    }

    private void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);

        foreach (var file in Directory.GetFiles(source))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(target, fileName);
            File.Copy(file, destFile, true);
        }

        foreach (var dir in Directory.GetDirectories(source))
        {
            string dirName = Path.GetFileName(dir);
            string destDir = Path.Combine(target, dirName);
            CopyDirectory(dir, destDir);
        }
    }

    private long CalculateTotalSize(string directory)
    {
        long totalSize = 0;

        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            totalSize += new FileInfo(file).Length;
        }

        return totalSize;
    }

    private int CountFilesRecursive(string directory)
    {
        return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Length;
    }

    #endregion
}
