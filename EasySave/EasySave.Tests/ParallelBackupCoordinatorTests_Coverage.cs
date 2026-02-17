using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using EasySave.Core.Services;
using EasySave.Shared;
using EasyLog;

namespace EasySave.Tests
{
    public class FakeLogger : ILogger
    {
        public List<LogEntry> Logs { get; } = new List<LogEntry>();

        public void WriteLog(LogEntry logEntry)
        {
            Logs.Add(logEntry);
        }

        public void UpdateState(StateEntry stateEntry)
        {
            // No-op
        }
    }

    public class ParallelBackupCoordinatorTests_Coverage
    {
        private readonly FakeLogger _fakeLogger;
        private readonly string _cryptoSoftPath = "dummy/path";
        private readonly List<string> _extensionsToEncrypt = new List<string>();

        public ParallelBackupCoordinatorTests_Coverage()
        {
            _fakeLogger = new FakeLogger();
        }

        [Fact]
        public void ResumeJob_ShouldBeBlocked_WhenBusinessSoftwareIsRunning()
        {
            // Arrange
            using (var testProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--info",
                UseShellExecute = false,
                CreateNoWindow = true
            }))
            {
                System.Threading.Thread.Sleep(500);
                try
                {
                    var businessSoftware = new List<string> { "dotnet" };
                    var coordinator = new ParallelBackupCoordinator(_fakeLogger, _cryptoSoftPath, _extensionsToEncrypt, businessSoftware);
                    var jobs = new List<BackupJob>
                    {
                        new BackupJob("TestJob_ResumeBlock", "src", "dst", BackupType.Full)
                    };

                    // Act
                    coordinator.ResumeJob("TestJob_ResumeBlock");

                    // Assert
                    // Should log "Resume blocked"
                    Assert.Contains(_fakeLogger.Logs, l => l.ErrorMessage.Contains("Resume blocked"));
                }
                finally
                {
                    testProcess?.Kill();
                    testProcess?.WaitForExit(1000);
                }
            }
        }
    }
}
