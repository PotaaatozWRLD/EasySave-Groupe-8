using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using EasySave.Core.Services;
using EasySave.Shared;
using EasyLog;

namespace EasySave.Tests
{
    public class ParallelBackupCoordinatorTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly string _cryptoSoftPath = "dummy/path";
        private readonly List<string> _extensionsToEncrypt = new List<string>();

        public ParallelBackupCoordinatorTests()
        {
            _loggerMock = new Mock<ILogger>();
        }

        [Fact]
        public async Task StartJobsAsync_ShouldStartAllJobs()
        {
            // Arrange
            var coordinator = new ParallelBackupCoordinator(_loggerMock.Object, _cryptoSoftPath, _extensionsToEncrypt);
            var jobs = new List<BackupJob>
            {
                new BackupJob("TestJob", "src", "dst", BackupType.Full)
            };

            // Act
            // StartJobsAsync is fire-and-forget but sets state synchronously
            await coordinator.StartJobsAsync(jobs);
            
            // Assert
            // We check if the job state was updated to Active, indicating it was accepted by the coordinator
            // Note: Since we don't mock the filesystem, the background task might fail quickly, 
            // but the initial state transition to "Active" happens before Task.Run.
            Assert.Equal("Active", jobs[0].State);
        }

        [Fact]
        public void PauseJob_ShouldNotThrow_WhenJobNotFound()
        {
            // Arrange
            var coordinator = new ParallelBackupCoordinator(_loggerMock.Object, _cryptoSoftPath, _extensionsToEncrypt);
            var jobName = "Job1";

            // Act & Assert
            // PauseJob returns void and handles missing jobs gracefully
            var exception = Record.Exception(() => coordinator.PauseJob(jobName));
            Assert.Null(exception);
        }
    }
}
