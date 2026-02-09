using EasySave.Core.Services;
using EasySave.Shared;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for JobManager class.
/// Tests job creation, deletion, retrieval, and max limit enforcement.
/// </summary>
public class JobManagerTests
{
    public JobManagerTests()
    {
        // Clear all jobs before each test to ensure isolation
        JobManager.LoadJobs();
        while (JobManager.Jobs.Count > 0)
        {
            JobManager.DeleteJob(0);
        }
    }

    [Fact]
    public void AddJob_ShouldAddJobSuccessfully()
    {
        // Arrange
        int initialCount = JobManager.Jobs.Count;

        // Act
        JobManager.AddJob("Test Backup", "C:\\Source", "D:\\Target", BackupType.Full);

        // Assert
        Assert.Equal(initialCount + 1, JobManager.Jobs.Count);
        Assert.Contains(JobManager.Jobs, j => j.Name == "Test Backup");
    }

    [Fact]
    public void AddJob_v2_ShouldAllowUnlimitedJobs()
    {
        // Arrange - v2.0 allows unlimited jobs (v1.0/v1.1 limited to 5)
        // Act - Add 20 jobs to verify no limit
        for (int i = 0; i < 20; i++)
        {
            JobManager.AddJob($"Job {i}", "C:\\Source", "D:\\Target", BackupType.Full);
        }

        // Assert - Should succeed with 20+ jobs
        Assert.True(JobManager.Jobs.Count >= 20, "v2.0 should allow unlimited jobs");
    }

    [Fact]
    public void DeleteJob_ShouldRemoveJobSuccessfully()
    {
        // Arrange
        JobManager.AddJob("To Delete", "C:\\Source", "D:\\Target", BackupType.Differential);
        int countBefore = JobManager.Jobs.Count;

        // Act
        JobManager.DeleteJob(JobManager.Jobs.Count - 1);

        // Assert
        Assert.Equal(countBefore - 1, JobManager.Jobs.Count);
        Assert.DoesNotContain(JobManager.Jobs, j => j.Name == "To Delete");
    }

    [Fact]
    public void DeleteJob_ShouldThrowException_WhenIndexInvalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => JobManager.DeleteJob(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => JobManager.DeleteJob(999));
    }

    [Fact]
    public void GetJob_ShouldReturnCorrectJob()
    {
        // Arrange
        JobManager.AddJob("Get Test", "C:\\Source", "D:\\Target", BackupType.Full);
        int lastIndex = JobManager.Jobs.Count - 1;

        // Act
        var job = JobManager.GetJob(lastIndex);

        // Assert
        Assert.NotNull(job);
        Assert.Equal("Get Test", job.Name);
        Assert.Equal(BackupType.Full, job.Type);
    }

    [Fact]
    public void GetJob_ShouldThrowException_WhenIndexInvalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => JobManager.GetJob(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => JobManager.GetJob(999));
    }

}
