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
        // Arrange - Start a calculator process for testing
        Process? testProcess = null;
        try
        {
            testProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "calc.exe",
                UseShellExecute = true,
                CreateNoWindow = false
            });

            // Wait longer for process to fully start and be queryable
            System.Threading.Thread.Sleep(2000);

            // Act - Windows 10+ uses CalculatorApp, not calc
            bool result = BusinessSoftwareDetector.IsRunning("CalculatorApp");

            // Assert
            Assert.True(result, "CalculatorApp should be detected as running");
        }
        finally
        {
            // Cleanup - Kill all CalculatorApp instances
            var calculators = Process.GetProcessesByName("CalculatorApp");
            foreach (var calc in calculators)
            {
                try
                {
                    calc.Kill();
                    calc.WaitForExit(1000);
                    calc.Dispose();
                }
                catch { /* Ignore cleanup errors */ }
            }
        }
    }

    [Fact]
    public void IsRunning_WithExeExtension_ShouldStillDetectProcess()
    {
        // Arrange - Start a calculator process for testing
        Process? testProcess = null;
        try
        {
            testProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "calc.exe",
                UseShellExecute = true,
                CreateNoWindow = false
            });

            // Wait longer for process to fully start and be queryable
            System.Threading.Thread.Sleep(2000);

            // Act - Pass name with .exe extension (Windows 10+ uses CalculatorApp)
            bool result = BusinessSoftwareDetector.IsRunning("CalculatorApp.exe");

            // Assert
            Assert.True(result, "CalculatorApp should be detected even with .exe extension");
        }
        finally
        {
            // Cleanup - Kill all CalculatorApp instances
            var calculators = Process.GetProcessesByName("CalculatorApp");
            foreach (var calc in calculators)
            {
                try
                {
                    calc.Kill();
                    calc.WaitForExit(1000);
                    calc.Dispose();
                }
                catch { /* Ignore cleanup errors */ }
            }
        }
    }

    [Fact]
    public void IsRunning_AfterProcessClosed_ShouldReturnFalse()
    {
        // Arrange - Start and immediately close a process
        Process? testProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "calc.exe",
            UseShellExecute = true,
            CreateNoWindow = false
        });

        System.Threading.Thread.Sleep(2000);

        // Act - Kill all calculators
        var calculators = Process.GetProcessesByName("CalculatorApp");
        foreach (var calc in calculators)
        {
            try
            {
                calc.Kill();
                calc.WaitForExit(1000);
                calc.Dispose();
            }
            catch { /* Ignore */ }
        }

        // Wait for cleanup
        System.Threading.Thread.Sleep(500);

        // Check if still running
        bool result = BusinessSoftwareDetector.IsRunning("CalculatorApp");

        // Assert
        Assert.False(result, "CalculatorApp should not be detected after being closed");
    }

    [Fact]
    public void IsRunning_CaseInsensitive_ShouldWork()
    {
        // Arrange - Start calculator
        Process? testProcess = null;
        try
        {
            testProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "calc.exe",
                UseShellExecute = true,
                CreateNoWindow = false
            });

            // Wait longer for process to fully start and be queryable
            System.Threading.Thread.Sleep(2000);

            // Act - Try different casings (Windows 10+ uses CalculatorApp)
            bool resultLowerCase = BusinessSoftwareDetector.IsRunning("calculatorapp");
            bool resultUpperCase = BusinessSoftwareDetector.IsRunning("CALCULATORAPP");
            bool resultMixedCase = BusinessSoftwareDetector.IsRunning("CalculatorApp");

            // Assert
            Assert.True(resultLowerCase, "Should detect with lowercase");
            Assert.True(resultUpperCase, "Should detect with uppercase");
            Assert.True(resultMixedCase, "Should detect with mixed case");
        }
        finally
        {
            // Cleanup - Kill all CalculatorApp instances
            var calculators = Process.GetProcessesByName("CalculatorApp");
            foreach (var calc in calculators)
            {
                try
                {
                    calc.Kill();
                    calc.WaitForExit(1000);
                    calc.Dispose();
                }
                catch { /* Ignore cleanup errors */ }
            }
        }
    }
}
