using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Fact]
        public async Task ResumeJob_ShouldBeBlocked_WhenBusinessSoftwareIsRunning()
        {
            // Arrange - Use a long-lived process for CI stability
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-Command \"Start-Sleep -Seconds 10\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var testProcess = Process.Start(startInfo))
            {
                // Wait for process to be fully up
                await Task.Delay(500);
                
                try
                {
                    var businessSoftware = new List<string> { "powershell" };
                    var coordinator = new ParallelBackupCoordinator(_loggerMock.Object, _cryptoSoftPath, _extensionsToEncrypt, businessSoftware);
                    
                    // Critical: Give the internal monitor time to perform its first check (initial delay 0 but async)
                    await Task.Delay(1000);

                    // Act
                    coordinator.ResumeJob("TestJob_ResumeBlock");

                    // Assert
                    // Should log "Resume blocked"
                    _loggerMock.Verify(l => l.WriteLog(It.Is<LogEntry>(e => (e.ErrorMessage ?? "").Contains("Resume blocked"))), Times.Once);
                }
                finally
                {
                    testProcess?.Kill();
                    testProcess?.WaitForExit(1000);
                }
            }
        }

        [Fact]
        public async Task StartJobsAsync_ShouldPauseOnStart_WhenBusinessSoftwareIsRunning()
        {
            // Arrange - Create real temp directories to avoid "Error" state in BackupService
            string tempBase = Path.Combine(Path.GetTempPath(), "EasySave_Test_" + Guid.NewGuid().ToString("N"));
            string src = Path.Combine(tempBase, "src");
            string dst = Path.Combine(tempBase, "dst");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(dst);
            File.WriteAllText(Path.Combine(src, "dummy.txt"), "content");

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-Command \"Start-Sleep -Seconds 10\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var testProcess = Process.Start(startInfo))
            {
                await Task.Delay(500);
                
                try
                {
                    var businessSoftware = new List<string> { "powershell" };
                    var coordinator = new ParallelBackupCoordinator(_loggerMock.Object, _cryptoSoftPath, _extensionsToEncrypt, businessSoftware);
                    
                    // Give monitor time to initialize
                    await Task.Delay(1000);

                    var jobs = new List<BackupJob>
                    {
                        new BackupJob("TestJob_PauseStart", src, dst, BackupType.Full)
                    };

                    // Act
                    await coordinator.StartJobsAsync(jobs);
                    
                    // Allow time for state transition
                    await Task.Delay(500);

                    // Assert
                    // The job should be in "Paused" state because business software is running
                    // If it's "Active" or "Error", it means the block failed.
                    Assert.Equal("Paused", jobs[0].State);
                    
                    // Verify log
                    _loggerMock.Verify(l => l.WriteLog(It.Is<LogEntry>(e => (e.ErrorMessage ?? "").Contains("Paused on start"))), Times.AtLeastOnce);
                }
                finally
                {
                    testProcess?.Kill();
                    testProcess?.WaitForExit(1000);
                    try { Directory.Delete(tempBase, true); } catch { /* cleanup is best effort */ }
                }
            }
        }
    }
}
