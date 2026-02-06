using EasySave.Shared;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for BackupJob class.
/// Tests job creation and property validation.
/// </summary>
public class BackupJobTests
{
    [Fact]
    public void BackupJob_ShouldInitializeWithValidData()
    {
        // Arrange & Act
        var job = new BackupJob(
            "Daily Backup",
            "C:\\Documents",
            "D:\\Backups\\Documents",
            BackupType.Full
        );

        // Assert
        Assert.Equal("Daily Backup", job.Name);
        Assert.Equal("C:\\Documents", job.SourcePath);
        Assert.Equal("D:\\Backups\\Documents", job.TargetPath);
        Assert.Equal(BackupType.Full, job.Type);
    }

    [Fact]
    public void BackupJob_ShouldSupportDifferentialType()
    {
        // Arrange & Act
        var job = new BackupJob(
            "Incremental Backup",
            "C:\\Projects",
            "E:\\Backups\\Projects",
            BackupType.Differential
        );

        // Assert
        Assert.Equal(BackupType.Differential, job.Type);
    }

    [Fact]
    public void BackupJob_ShouldSupportNetworkPaths()
    {
        // Arrange & Act
        var job = new BackupJob(
            "Network Backup",
            "\\\\server\\share\\data",
            "\\\\backup\\archive\\data",
            BackupType.Full
        );

        // Assert
        Assert.StartsWith("\\\\", job.SourcePath);
        Assert.StartsWith("\\\\", job.TargetPath);
    }

    [Fact]
    public void BackupJob_PropertiesShouldBeSettable()
    {
        // Arrange
        var job = new BackupJob("Test", "C:\\A", "D:\\B", BackupType.Full);

        // Act
        job.Name = "Updated Test";
        job.SourcePath = "C:\\Updated";
        job.TargetPath = "D:\\Updated";
        job.Type = BackupType.Differential;

        // Assert
        Assert.Equal("Updated Test", job.Name);
        Assert.Equal("C:\\Updated", job.SourcePath);
        Assert.Equal("D:\\Updated", job.TargetPath);
        Assert.Equal(BackupType.Differential, job.Type);
    }
}
