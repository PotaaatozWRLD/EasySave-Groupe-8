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
        public void GetFirstRunningProcess_ShouldReturnProcessName_WhenProcessIsRunning()
        {
            // Arrange - Start a dummy process
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
                    var processes = new List<string> { "NonExistentProcess", "dotnet" };

                    // Act
                    string? result = BusinessSoftwareDetector.GetFirstRunningProcess(processes);

                    // Assert
                    Assert.Equal("dotnet", result);
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
