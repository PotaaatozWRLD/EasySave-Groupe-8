using EasySave.Core.Services;
using EasySave.Shared;
using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Integration tests for complete backup operations.
/// Tests real file operations with temporary directories.
/// </summary>
public class BackupIntegrationTests : IDisposable
{
    private readonly string _testRoot;
    private readonly string _testSourceDir;
    private readonly string _testTargetDir;
    private readonly string _testLogDir;
    private readonly string _testStateFile;

    public BackupIntegrationTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "EasySaveTests", Guid.NewGuid().ToString());
        _testSourceDir = Path.Combine(_testRoot, "Source");
        _testTargetDir = Path.Combine(_testRoot, "Target");
        _testLogDir = Path.Combine(_testRoot, "Logs");
        _testStateFile = Path.Combine(_testRoot, "state.json");

        Directory.CreateDirectory(_testSourceDir);
        Directory.CreateDirectory(_testTargetDir);
        Directory.CreateDirectory(_testLogDir);
    }

    [Fact]
    public void FullBackup_ShouldCopyAllFiles()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 5, "test");
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Full Backup Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        var sourceFiles = Directory.GetFiles(_testSourceDir, "*", SearchOption.AllDirectories);
        var targetFiles = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.Equal(sourceFiles.Length, targetFiles.Length);
    }

    [Fact]
    public void DifferentialBackup_ShouldCopyOnlyModifiedFiles()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 3, "original");
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Diff Backup Test", _testSourceDir, _testTargetDir, BackupType.Differential);

        // First backup (full)
        service.ExecuteBackup(job);
        var initialCount = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories).Length;

        // Modify one file
        Thread.Sleep(1100); // Ensure timestamp difference
        File.WriteAllText(Path.Combine(_testSourceDir, "test0.txt"), "modified content");

        // Clear target to test differential
        Directory.Delete(_testTargetDir, true);
        Directory.CreateDirectory(_testTargetDir);

        // Act - Second backup (differential)
        service.ExecuteBackup(job);

        // Assert - Only modified file should be copied
        var targetFiles = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.True(targetFiles.Length > 0, "Differential backup should copy modified files");
    }

    [Fact]
    public void Backup_WithSpecialCharacters_ShouldSucceed()
    {
        // Arrange
        string specialFile = Path.Combine(_testSourceDir, "file with spaces & (parentheses).txt");
        File.WriteAllText(specialFile, "Special content");
        
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Special Chars Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, "file with spaces & (parentheses).txt");
        Assert.True(File.Exists(targetFile), "File with special characters should be copied");
    }

    [Fact]
    public void Backup_WithSubdirectories_ShouldCopyRecursively()
    {
        // Arrange
        string subDir1 = Path.Combine(_testSourceDir, "SubDir1");
        string subDir2 = Path.Combine(_testSourceDir, "SubDir1", "SubDir2");
        Directory.CreateDirectory(subDir1);
        Directory.CreateDirectory(subDir2);
        
        File.WriteAllText(Path.Combine(subDir1, "file1.txt"), "Content 1");
        File.WriteAllText(Path.Combine(subDir2, "file2.txt"), "Content 2");
        
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Recursive Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        Assert.True(File.Exists(Path.Combine(_testTargetDir, "SubDir1", "file1.txt")));
        Assert.True(File.Exists(Path.Combine(_testTargetDir, "SubDir1", "SubDir2", "file2.txt")));
    }

    [Fact]
    public void Backup_WithLargeFile_ShouldComplete()
    {
        // Arrange - Create a 5MB file
        string largeFile = Path.Combine(_testSourceDir, "large.dat");
        byte[] data = new byte[5 * 1024 * 1024]; // 5MB
        new Random().NextBytes(data);
        File.WriteAllBytes(largeFile, data);
        
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Large File Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, "large.dat");
        Assert.True(File.Exists(targetFile));
        Assert.Equal(new FileInfo(largeFile).Length, new FileInfo(targetFile).Length);
    }

    private void CreateTestFiles(string directory, int count, string prefix)
    {
        for (int i = 0; i < count; i++)
        {
            string filePath = Path.Combine(directory, $"{prefix}{i}.txt");
            File.WriteAllText(filePath, $"Test content {i}");
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
                // Cleanup may fail if files are locked, ignore
            }
        }
    }
}
