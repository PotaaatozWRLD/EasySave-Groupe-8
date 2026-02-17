using Xunit;
using EasySave.Core.Services;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace EasySave.Tests
{
    public class BusinessSoftwareDetectorTests_Coverage
    {
        [Fact]
        public void GetFirstRunningProcess_ShouldReturnNull_WhenListIsNull()
        {
            // Act
            string? result = BusinessSoftwareDetector.GetFirstRunningProcess(null!);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFirstRunningProcess_ShouldReturnNull_WhenListIsEmpty()
        {
            // Act
            string? result = BusinessSoftwareDetector.GetFirstRunningProcess(new List<string>());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFirstRunningProcess_ShouldReturnNull_WhenNoProcessIsRunning()
        {
            // Arrange
            var processes = new List<string> { "NonExistentProcess_12345", "AnotherFakeProcess_67890" };

            // Act
            string? result = BusinessSoftwareDetector.GetFirstRunningProcess(processes);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFirstRunningProcess_ShouldReturnProcessName_WhenProcessIsRunning()
        {
            // Arrange - Start a dummy process (PowerShell sleep for stability)
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-Command \"Start-Sleep -Seconds 5\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var testProcess = Process.Start(startInfo))
            {
                // Give it a moment to start
                await Task.Delay(500);
                try
                {
                    var processes = new List<string> { "NonExistentProcess", "powershell" };

                    // Act
                    string? result = BusinessSoftwareDetector.GetFirstRunningProcess(processes);

                    // Assert
                    Assert.Equal("powershell", result);
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
