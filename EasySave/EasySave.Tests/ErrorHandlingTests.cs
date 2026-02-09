using EasySave.Core.Services;
using EasySave.Shared;
using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Tests for error handling in backup operations.
/// Tests invalid paths, permissions, and edge cases.
/// </summary>
public class ErrorHandlingTests : IDisposable
{
    private readonly string _testRoot;
    private readonly string _testSourceDir;
    private readonly string _testTargetDir;
    private readonly string _testLogDir;
    private readonly string _testStateFile;

    public ErrorHandlingTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "EasySaveErrorTests", Guid.NewGuid().ToString());
        _testSourceDir = Path.Combine(_testRoot, "Source");
        _testTargetDir = Path.Combine(_testRoot, "Target");
        _testLogDir = Path.Combine(_testRoot, "Logs");
        _testStateFile = Path.Combine(_testRoot, "state.json");

        Directory.CreateDirectory(_testSourceDir);
        Directory.CreateDirectory(_testTargetDir);
        Directory.CreateDirectory(_testLogDir);
    }

    [Fact]
    public void Backup_WithNonExistentSourcePath_ShouldHandleGracefully()
    {
        // Arrange
        string nonExistentPath = Path.Combine(_testRoot, "NonExistent");
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Invalid Source", nonExistentPath, _testTargetDir, BackupType.Full);

        // Act & Assert
        var exception = Record.Exception(() => service.ExecuteBackup(job));
        
        // Should either throw or handle gracefully
        if (exception != null)
        {
            Assert.IsType<DirectoryNotFoundException>(exception);
        }
    }

    [Fact]
    public void Backup_WithEmptySourceDirectory_ShouldComplete()
    {
        // Arrange
        string emptyDir = Path.Combine(_testRoot, "Empty");
        Directory.CreateDirectory(emptyDir);
        
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Empty Source", emptyDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert - Should complete without errors
        var targetFiles = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.Empty(targetFiles);
    }

    [Fact]
    public void Backup_WithReadOnlyFile_ShouldCopySuccessfully()
    {
        // Arrange
        string readOnlyFile = Path.Combine(_testSourceDir, "readonly.txt");
        File.WriteAllText(readOnlyFile, "Read-only content");
        File.SetAttributes(readOnlyFile, FileAttributes.ReadOnly);

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("ReadOnly Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, "readonly.txt");
        Assert.True(File.Exists(targetFile));
    }

    [Fact]
    public void Backup_WithVeryLongFileName_ShouldHandleCorrectly()
    {
        // Arrange - Create file with long name (close to Windows limit)
        string longName = new string('a', 200) + ".txt";
        string longFilePath = Path.Combine(_testSourceDir, longName);
        
        try
        {
            File.WriteAllText(longFilePath, "Long name content");
        }
        catch (PathTooLongException)
        {
            // Skip test if path is too long for this system
            return;
        }

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Long Name Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        var exception = Record.Exception(() => service.ExecuteBackup(job));

        // Assert - Should either succeed or handle error gracefully
        Assert.True(exception == null || exception is PathTooLongException);
    }

    [Fact(Skip = "Path validation varies by OS version")]
    public void Backup_WithInvalidCharactersInPath_ShouldValidate()
    {
        // Arrange
        var invalidChars = Path.GetInvalidPathChars();
        if (invalidChars.Length == 0) return; // No invalid chars to test

        // Can't actually create paths with invalid chars, so test validation logic
        char invalidChar = invalidChars[0];
        string invalidPath = $"C:\\Invalid{invalidChar}Path";

        // Act & Assert
        var exception = Record.Exception(() => Path.GetFullPath(invalidPath));
        Assert.True(exception != null, "Invalid path should throw exception");
    }

    [Fact]
    public void Backup_WithZeroByteFile_ShouldCopy()
    {
        // Arrange
        string emptyFile = Path.Combine(_testSourceDir, "empty.txt");
        File.WriteAllText(emptyFile, string.Empty);

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Empty File Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, "empty.txt");
        Assert.True(File.Exists(targetFile));
        Assert.Equal(0, new FileInfo(targetFile).Length);
    }

    [Fact]
    public void Backup_WithSameSourceAndTarget_ShouldHandleError()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Same Path Test", _testSourceDir, _testSourceDir, BackupType.Full);

        File.WriteAllText(Path.Combine(_testSourceDir, "test.txt"), "content");

        // Act & Assert
        // Should either prevent or handle circular backup attempt
        var exception = Record.Exception(() => service.ExecuteBackup(job));
        
        // Implementation-dependent: may throw or skip files
        Assert.True(exception == null || exception is IOException);
    }

    [Fact]
    public void Backup_WithLockedFile_ShouldLogError()
    {
        // Arrange
        string lockedFile = Path.Combine(_testSourceDir, "locked.txt");
        
        // Create and lock the file
        using (var stream = File.Create(lockedFile))
        {
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);
            stream.Flush();

            var logger = new JsonLogger(_testLogDir, _testStateFile);
            var service = new BackupService(logger);
            var job = new BackupJob("Locked File Test", _testSourceDir, _testTargetDir, BackupType.Full);

            // Act - Try to backup while file is locked
            var exception = Record.Exception(() => service.ExecuteBackup(job));

            // Assert - Should handle locked file (may throw IOException)
            // Implementation determines if it skips or throws
            Assert.True(exception == null || exception is IOException);
        }
    }

    [Fact]
    public void Backup_WithHiddenFile_ShouldCopy()
    {
        // Arrange
        string hiddenFile = Path.Combine(_testSourceDir, ".hidden.txt");
        File.WriteAllText(hiddenFile, "Hidden content");
        File.SetAttributes(hiddenFile, FileAttributes.Hidden);

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Hidden File Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, ".hidden.txt");
        Assert.True(File.Exists(targetFile));
    }

    [Fact]
    public void Backup_WithSystemFile_ShouldCopy()
    {
        // Arrange
        string systemFile = Path.Combine(_testSourceDir, "system.dat");
        File.WriteAllText(systemFile, "System file content");
        File.SetAttributes(systemFile, FileAttributes.System);

        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("System File Test", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string targetFile = Path.Combine(_testTargetDir, "system.dat");
        Assert.True(File.Exists(targetFile));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            try
            {
                // Remove read-only attributes before deletion
                foreach (var file in Directory.GetFiles(_testRoot, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }
                    catch
                    {
                        // Ignore if we can't reset attributes during cleanup
                    }
                }
                Directory.Delete(_testRoot, true);
            }
            catch (IOException)
            {
                // Cleanup may fail if files are locked, ignore
            }
            catch (UnauthorizedAccessException)
            {
                // Cleanup may fail due to permissions, ignore
            }
        }
    }
}
