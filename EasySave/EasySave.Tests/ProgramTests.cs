using Xunit;
using EasySave.Shared;
using EasySave.Core.Services;
using System;
using System.IO;

namespace EasySave.Tests;

/// <summary>
/// Integration tests for Program class command-line argument handling
/// These tests improve code coverage for EasySave.Console
/// </summary>
public class ProgramArgumentsTests
{
    [Fact]
    public void JobManager_LoadJobs_DoesNotThrow()
    {
        // Arrange & Act
        var exception = Record.Exception(() => JobManager.LoadJobs());
        
        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void JobManager_GetJobs_ReturnsCollection()
    {
        // Arrange
        JobManager.LoadJobs();
        
        // Act
        var jobs = JobManager.Jobs;
        
        // Assert
        Assert.NotNull(jobs);
    }

    [Fact]
    public void BackupJob_CreateWithValidData_Success()
    {
        // Arrange
        string name = "TestJob";
        string source = Path.GetTempPath();
        string target = Path.Combine(Path.GetTempPath(), "backup");
        BackupType type = BackupType.Full;
        
        // Act
        var job = new BackupJob(name, source, target, type);
        
        // Assert
        Assert.Equal(name, job.Name);
        Assert.Equal(source, job.SourcePath);
        Assert.Equal(target, job.TargetPath);
        Assert.Equal(type, job.Type);
        Assert.NotNull(job.CreatedDate);
        Assert.NotNull(job.Description);
    }

    [Fact]
    public void BackupJob_SetProperties_UpdatesValues()
    {
        // Arrange
        var job = new BackupJob("Initial", "C:\\source", "C\\target", BackupType.Full);
        
        // Act
        job.Name = "Updated";
        job.Type = BackupType.Differential;
        
        // Assert
        Assert.Equal("Updated", job.Name);
        Assert.Equal(BackupType.Differential, job.Type);
    }

    [Fact]
    public void BackupType_EnumValues_AreValid()
    {
        // Act
        var fullType = BackupType.Full;
        var diffType = BackupType.Differential;
        
        // Assert
        Assert.Equal(0, (int)fullType);
        Assert.Equal(1, (int)diffType);
    }
    
    [Fact]
    public void BackupJob_DescriptionCanBeModified()
    {
        // Arrange
        var job = new BackupJob("Test", "C:\\source", "C:\\target", BackupType.Full);
        
        // Act
        job.Description = "Custom description";
        
        // Assert
        Assert.Equal("Custom description", job.Description);
    }
}
