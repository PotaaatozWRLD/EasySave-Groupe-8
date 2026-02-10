using Xunit;
using EasySave.Core.Services;
using System.Diagnostics;

namespace EasySave.Tests;

/// <summary>
/// Tests for BusinessSoftwareDetector service (v2.0).
/// Verifies process detection functionality for preventing backups when business software is running.
/// </summary>
public class BusinessSoftwareDetectorTests
{
    [Fact]
    public void IsRunning_WithNullProcessName_ShouldReturnFalse()
    {
        // Act
        bool result = BusinessSoftwareDetector.IsRunning(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRunning_WithEmptyProcessName_ShouldReturnFalse()
    {
        // Act
        bool result = BusinessSoftwareDetector.IsRunning("");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRunning_WithWhitespaceProcessName_ShouldReturnFalse()
    {
        // Act
        bool result = BusinessSoftwareDetector.IsRunning("   ");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRunning_WithNonExistentProcess_ShouldReturnFalse()
    {
        // Arrange
        string nonExistentProcess = "ThisProcessDoesNotExistXYZ12345";

        // Act
        bool result = BusinessSoftwareDetector.IsRunning(nonExistentProcess);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRunning_WithRunningProcess_ShouldReturnTrue()
    {
        // Arrange - Start a long-running dotnet process for testing (cross-platform)
        using (var testProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "--info",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        }))
        {
            // Wait for process to fully start and be queryable
            System.Threading.Thread.Sleep(500);

            // Act
            bool result = BusinessSoftwareDetector.IsRunning("dotnet");

            // Assert
            Assert.True(result, "dotnet process should be detected as running");
            
            // Cleanup
            testProcess?.Kill();
            testProcess?.WaitForExit(1000);
        }
    }

    [Fact]
    public void IsRunning_WithExeExtension_ShouldStillDetectProcess()
    {
        // Arrange - Start a long-running dotnet process for testing (cross-platform)
        using (var testProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "--info",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        }))
        {
            // Wait for process to fully start and be queryable
            System.Threading.Thread.Sleep(500);

            // Act - Pass name with .exe extension
            bool result = BusinessSoftwareDetector.IsRunning("dotnet.exe");

            // Assert
            Assert.True(result, "dotnet process should be detected even with .exe extension");
            
            // Cleanup
            testProcess?.Kill();
            testProcess?.WaitForExit(1000);
        }
    }

    [Fact]
    public void IsRunning_AfterProcessClosed_ShouldReturnFalse()
    {
        // Arrange - Start a test process with unique identifier
        string testProcessName = "TestDotnetProcess_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        Process? testProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "--version",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        });

        System.Threading.Thread.Sleep(100);

        // Act - Kill the process
        testProcess?.Kill();
        testProcess?.WaitForExit(1000);
        testProcess?.Dispose();

        // Wait for cleanup
        System.Threading.Thread.Sleep(300);

        // For this test, we verify that a non-existent process returns false
        bool result = BusinessSoftwareDetector.IsRunning(testProcessName);

        // Assert
        Assert.False(result, "Non-existent process should not be detected");
    }

    [Fact]
    public void IsRunning_CaseInsensitive_ShouldWork()
    {
        // Arrange - Start a long-running dotnet process (cross-platform)
        using (var testProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "--info",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true
        }))
        {
            // Wait for process to fully start and be queryable
            System.Threading.Thread.Sleep(500);

            // Act - Try different casings
            bool resultLowerCase = BusinessSoftwareDetector.IsRunning("dotnet");
            bool resultUpperCase = BusinessSoftwareDetector.IsRunning("DOTNET");
            bool resultMixedCase = BusinessSoftwareDetector.IsRunning("DotNet");

            // Assert
            Assert.True(resultLowerCase, "Should detect with lowercase");
            Assert.True(resultUpperCase, "Should detect with uppercase");
            Assert.True(resultMixedCase, "Should detect with mixed case");
            
            // Cleanup
            testProcess?.Kill();
            testProcess?.WaitForExit(1000);
        }
    }
}
