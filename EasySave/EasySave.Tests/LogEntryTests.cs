using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for LogEntry class.
/// Tests all required fields for daily log entries.
/// </summary>
public class LogEntryTests
{
    [Fact]
    public void LogEntry_ShouldInitializeWithAllFields()
    {
        // Arrange & Act
        var entry = new LogEntry
        {
            JobName = "Test Backup",
            SourcePath = "\\\\server\\source\\file.txt",
            TargetPath = "\\\\server\\target\\file.txt",
            FileName = "file.txt",
            FileSize = 1024,
            TransferTime = 150,
            ErrorMessage = null,
            Timestamp = DateTime.Now
        };

        // Assert
        Assert.Equal("Test Backup", entry.JobName);
        Assert.Equal("\\\\server\\source\\file.txt", entry.SourcePath);
        Assert.Equal("\\\\server\\target\\file.txt", entry.TargetPath);
        Assert.Equal("file.txt", entry.FileName);
        Assert.Equal(1024, entry.FileSize);
        Assert.Equal(150, entry.TransferTime);
        Assert.Null(entry.ErrorMessage);
        Assert.NotEqual(default(DateTime), entry.Timestamp);
    }

    [Fact]
    public void LogEntry_ShouldHandleErrorWithNegativeTransferTime()
    {
        // Arrange & Act
        var entry = new LogEntry
        {
            JobName = "Failed Backup",
            SourcePath = "C:\\source\\file.txt",
            TargetPath = "D:\\target\\file.txt",
            FileName = "file.txt",
            FileSize = 2048,
            TransferTime = -1,
            ErrorMessage = "Access denied",
            Timestamp = DateTime.Now
        };

        // Assert
        Assert.True(entry.TransferTime < 0, "Transfer time should be negative for errors");
        Assert.NotNull(entry.ErrorMessage);
        Assert.Equal("Access denied", entry.ErrorMessage);
    }

    [Fact]
    public void LogEntry_ShouldSupportUNCPaths()
    {
        // Arrange & Act
        var entry = new LogEntry
        {
            JobName = "Network Backup",
            SourcePath = "\\\\192.168.1.100\\share\\data",
            TargetPath = "\\\\backup-server\\backups\\data",
            FileName = "data.xml",
            FileSize = 512,
            TransferTime = 75,
            Timestamp = DateTime.Now
        };

        // Assert
        Assert.StartsWith("\\\\", entry.SourcePath);
        Assert.StartsWith("\\\\", entry.TargetPath);
    }
}
