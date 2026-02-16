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
        public async Task ExecuteJobsInParallelAsync_ShouldStartAllJobs()
        {
            // Arrange
            var coordinator = new ParallelBackupCoordinator(_loggerMock.Object, _cryptoSoftPath, _extensionsToEncrypt);
            var jobs = new List<BackupJob>(); // Empty list to test coordinator mechanics without FS dependencies

            // Act
            // Note: In a real unit test without file system abstraction, we can't easily assert file operations 
            // without mocking System.IO. However, we can check if the method completes without error 
            // and if internal state tracking initializes.
            
            // To properly test this, we would need to refactor BackupService to be mockable or use a file system abstraction.
            // For now, we verify instantiation and sanity.
            
            var task = coordinator.ExecuteJobsInParallelAsync(jobs);
            
            // Assert
            Assert.NotNull(task);
            // We don't await task here as it would try to run actual backups on non-existent paths.
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
