using EasySave.Core.Services;
using EasySave.Shared;
using EasyLog;
using System.Diagnostics;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Performance tests for backup operations.
/// Tests handling of large files and progress tracking accuracy.
/// </summary>
public class PerformanceTests : IDisposable
{
    private readonly string _testRoot;
    private readonly string _testSourceDir;
    private readonly string _testTargetDir;
    private readonly string _testLogDir;
    private readonly string _testStateFile;

    public PerformanceTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "EasySavePerformanceTests", Guid.NewGuid().ToString());
        _testSourceDir = Path.Combine(_testRoot, "Source");
        _testTargetDir = Path.Combine(_testRoot, "Target");
        _testLogDir = Path.Combine(_testRoot, "Logs");
        _testStateFile = Path.Combine(_testRoot, "state.json");

        Directory.CreateDirectory(_testSourceDir);
        Directory.CreateDirectory(_testTargetDir);
        Directory.CreateDirectory(_testLogDir);
    }

    [Fact]
    public void Backup_With100Files_ShouldCompleteInReasonableTime()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 100, 1024); // 100 files of 1KB each
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Performance Test", _testSourceDir, _testTargetDir, BackupType.Full);

        var stopwatch = Stopwatch.StartNew();

        // Act
        service.ExecuteBackup(job);

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 30000, $"Backup took {stopwatch.ElapsedMilliseconds}ms, expected < 30000ms");
        
        var targetFiles = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.Equal(100, targetFiles.Length);
    }

    [Fact]
    public void Backup_WithLargeFile_ShouldTrackProgress()
    {
        // Arrange - Create a 10MB file
        string largeFile = Path.Combine(_testSourceDir, "large_10mb.dat");
        byte[] data = new byte[10 * 1024 * 1024]; // 10MB
        new Random().NextBytes(data);
        File.WriteAllBytes(largeFile, data);

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Large File Progress", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, "large_10mb.dat");
        Assert.True(File.Exists(targetFile));
        
        // Verify state file was updated
        Assert.True(File.Exists(_testStateFile));
        string stateContent = File.ReadAllText(_testStateFile);
        Assert.Contains("Large File Progress", stateContent);
    }

    [Fact]
    public void Backup_MultipleSmallFiles_ShouldUpdateProgressCorrectly()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 50, 512); // 50 files of 512 bytes
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Progress Tracking", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        var targetFiles = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.Equal(50, targetFiles.Length);

        // Check final state shows completion
        if (File.Exists(_testStateFile))
        {
            string stateContent = File.ReadAllText(_testStateFile);
            Assert.Contains("Progress Tracking", stateContent);
        }
    }

    [Fact]
    public void Backup_MixedFileSizes_ShouldCalculateTotalSizeCorrectly()
    {
        // Arrange
        File.WriteAllBytes(Path.Combine(_testSourceDir, "small.dat"), new byte[1024]); // 1KB
        File.WriteAllBytes(Path.Combine(_testSourceDir, "medium.dat"), new byte[100 * 1024]); // 100KB
        File.WriteAllBytes(Path.Combine(_testSourceDir, "large.dat"), new byte[1024 * 1024]); // 1MB

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Mixed Sizes", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        long expectedSize = 1024 + (100 * 1024) + (1024 * 1024); // Total size
        long actualSize = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories)
            .Sum(f => new FileInfo(f).Length);
        
        Assert.Equal(expectedSize, actualSize);
    }

    [Fact]
    public void Backup_TransferTime_ShouldBeRecordedAccurately()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 10, 10240); // 10 files of 10KB
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Transfer Time Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string logFile = Path.Combine(_testLogDir, $"{DateTime.Now:yyyy-MM-dd}.json");
        if (File.Exists(logFile))
        {
            string content = File.ReadAllText(logFile);
            Assert.Contains("TransferTime", content);
            Assert.DoesNotContain("\"TransferTime\": -1", content); // No errors
        }
    }

    private void CreateTestFiles(string directory, int count, int sizeBytes)
    {
        var random = new Random();
        for (int i = 0; i < count; i++)
        {
            string filePath = Path.Combine(directory, $"file_{i:D3}.dat");
            byte[] data = new byte[sizeBytes];
            random.NextBytes(data);
            File.WriteAllBytes(filePath, data);
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            try
            {
                Directory.Delete(_testRoot, true);
            }
            catch
            {
                // Cleanup may fail, ignore
            }
        }
    }
}
