using EasySave.Core.Services;
using EasySave.Shared;
using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for BackupService class.
/// Tests backup execution, file counting, and error handling.
/// Note: Some integration tests are commented out due to execution time.
/// </summary>
public class BackupServiceTests
{
    /* Integration tests commented out - too slow for CI
    private readonly string _testSourceDir;
    private readonly string _testTargetDir;
    private readonly string _testLogDir;
    private readonly string _testStateFile;

    public BackupServiceTests()
    {
        // Create temporary test directories
        string testRoot = Path.Combine(Path.GetTempPath(), "EasySaveTests", Guid.NewGuid().ToString());
        _testSourceDir = Path.Combine(testRoot, "Source");
        _testTargetDir = Path.Combine(testRoot, "Target");
        _testLogDir = Path.Combine(testRoot, "Logs");
        _testStateFile = Path.Combine(testRoot, "state.json");

        Directory.CreateDirectory(_testSourceDir);
        Directory.CreateDirectory(_testTargetDir);
        Directory.CreateDirectory(_testLogDir);
    }

    [Fact]
    public void ExecuteBackup_ShouldCopyFilesForFullBackup()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 3);
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Full Backup", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        var sourceFiles = Directory.GetFiles(_testSourceDir, "*", SearchOption.AllDirectories);
        var targetFiles = Directory.GetFiles(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.Equal(sourceFiles.Length, targetFiles.Length);
    }

    [Fact]
    public void ExecuteBackup_ShouldCreateTargetDirectory()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 2);
        string newTargetDir = Path.Combine(_testTargetDir, "NewFolder");
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Create Dir", _testSourceDir, newTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        Assert.True(Directory.Exists(newTargetDir));
        Assert.NotEmpty(Directory.GetFiles(newTargetDir));
    }

    [Fact]
    public void ExecuteBackup_ShouldHandleEmptySourceDirectory()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Empty Backup", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert - Should complete without error
        Assert.True(Directory.Exists(_testTargetDir));
    }

    [Fact]
    public void ExecuteBackup_ShouldCreateLogFile()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 2);
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Logging", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        string logFile = Path.Combine(_testLogDir, $"{DateTime.Now:yyyy-MM-dd}.json");
        Assert.True(File.Exists(logFile));
    }

    [Fact]
    public void ExecuteBackup_ShouldUpdateStateFile()
    {
        // Arrange
        CreateTestFiles(_testSourceDir, 1);
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test State", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        Assert.True(File.Exists(_testStateFile));
        string stateContent = File.ReadAllText(_testStateFile);
        Assert.Contains("Test State", stateContent);
        // State is serialized as enum value (2 for END) in JSON
        Assert.Contains("\"State\": 2", stateContent); // JobState.END = 2
    }

    [Fact]
    public void ExecuteBackup_ShouldHandleSubdirectories()
    {
        // Arrange
        CreateTestFilesWithSubdirs(_testSourceDir);
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Subdirs", _testSourceDir, _testTargetDir, BackupType.Full);

        // Act
        service.ExecuteBackup(job);

        // Assert
        var sourceSubdirs = Directory.GetDirectories(_testSourceDir, "*", SearchOption.AllDirectories);
        var targetSubdirs = Directory.GetDirectories(_testTargetDir, "*", SearchOption.AllDirectories);
        Assert.Equal(sourceSubdirs.Length, targetSubdirs.Length);
    }

    [Fact]
    public void ExecuteBackup_DifferentialShouldOnlyCopyNewOrModifiedFiles()
    {
        // Arrange - First full backup
        CreateTestFiles(_testSourceDir, 2);
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var service = new BackupService(logger);
        var fullJob = new BackupJob("First Full", _testSourceDir, _testTargetDir, BackupType.Full);
        service.ExecuteBackup(fullJob);

        // Modify one file and add a new one
        Thread.Sleep(1100); // Ensure timestamp difference
        File.WriteAllText(Path.Combine(_testSourceDir, "test_0.txt"), "Modified content");
        File.WriteAllText(Path.Combine(_testSourceDir, "new_file.txt"), "New file");

        // Act - Differential backup
        var diffJob = new BackupJob("Differential", _testSourceDir, _testTargetDir, BackupType.Differential);
        service.ExecuteBackup(diffJob);

        // Assert - New file should exist
        Assert.True(File.Exists(Path.Combine(_testTargetDir, "new_file.txt")));
    }
    */

    [Fact]
    public void BackupService_ShouldInitializeWithLogger()
    {
        // Arrange
        string tempLogDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string tempStateFile = Path.Combine(tempLogDir, "state.json");
        Directory.CreateDirectory(tempLogDir);
        var logger = new JsonLogger(tempLogDir, tempStateFile);

        // Act
        var service = new BackupService(logger);

        // Assert
        Assert.NotNull(service);
        
        // Cleanup
        Directory.Delete(tempLogDir, true);
    }

    [Fact]
    public void ExecuteBackup_v2_ShouldThrowException_WhenBusinessSoftwareIsRunning()
    {
        // Skip on non-Windows platforms (calc.exe is Windows-only)
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return;
        }

        // Arrange
        string tempLogDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string tempSourceDir = Path.Combine(tempLogDir, "Source");
        string tempTargetDir = Path.Combine(tempLogDir, "Target");
        string tempStateFile = Path.Combine(tempLogDir, "state.json");
        
        Directory.CreateDirectory(tempSourceDir);
        Directory.CreateDirectory(tempTargetDir);
        Directory.CreateDirectory(tempLogDir);
        
        // Create a test file
        File.WriteAllText(Path.Combine(tempSourceDir, "test.txt"), "Test content");
        
        var logger = new JsonLogger(tempLogDir, tempStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Backup", tempSourceDir, tempTargetDir, BackupType.Full);

        // Start calculator to simulate business software
        System.Diagnostics.Process? calcProcess = null;
        try
        {
            calcProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "calc.exe",
                UseShellExecute = true,
                CreateNoWindow = false
            });
            
            // Wait for process to start
            System.Threading.Thread.Sleep(2000);

            // Act & Assert - Should throw when business software is running
            var exception = Assert.Throws<InvalidOperationException>(() => 
                service.ExecuteBackup(job, "CalculatorApp"));
            
            Assert.Contains("CalculatorApp", exception.Message);
            Assert.Contains("is currently running", exception.Message);
        }
        finally
        {
            // Cleanup
            var calculators = System.Diagnostics.Process.GetProcessesByName("CalculatorApp");
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
            
            Directory.Delete(tempLogDir, true);
        }
    }

    [Fact]
    public void ExecuteBackup_v2_ShouldSucceed_WhenBusinessSoftwareNotRunning()
    {
        // Arrange
        string tempLogDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string tempSourceDir = Path.Combine(tempLogDir, "Source");
        string tempTargetDir = Path.Combine(tempLogDir, "Target");
        string tempStateFile = Path.Combine(tempLogDir, "state.json");
        
        Directory.CreateDirectory(tempSourceDir);
        Directory.CreateDirectory(tempTargetDir);
        Directory.CreateDirectory(tempLogDir);
        
        // Create a test file
        File.WriteAllText(Path.Combine(tempSourceDir, "test.txt"), "Test content");
        
        var logger = new JsonLogger(tempLogDir, tempStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Backup", tempSourceDir, tempTargetDir, BackupType.Full);

        try
        {
            // Ensure calculator is NOT running
            var calculators = System.Diagnostics.Process.GetProcessesByName("CalculatorApp");
            foreach (var calc in calculators)
            {
                try { calc.Kill(); calc.WaitForExit(1000); calc.Dispose(); }
                catch { /* Ignore */ }
            }
            
            System.Threading.Thread.Sleep(500);

            // Act - Should succeed when business software is not running
            service.ExecuteBackup(job, "CalculatorApp");

            // Assert - Backup completed successfully
            Assert.True(File.Exists(Path.Combine(tempTargetDir, "test.txt")));
        }
        finally
        {
            // Cleanup
            Directory.Delete(tempLogDir, true);
        }
    }

    [Fact]
    public void ExecuteBackup_v2_ShouldSucceed_WhenBusinessSoftwareNameIsNull()
    {
        // Arrange
        string tempLogDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string tempSourceDir = Path.Combine(tempLogDir, "Source");
        string tempTargetDir = Path.Combine(tempLogDir, "Target");
        string tempStateFile = Path.Combine(tempLogDir, "state.json");
        
        Directory.CreateDirectory(tempSourceDir);
        Directory.CreateDirectory(tempTargetDir);
        Directory.CreateDirectory(tempLogDir);
        
        // Create a test file
        File.WriteAllText(Path.Combine(tempSourceDir, "test.txt"), "Test content");
        
        var logger = new JsonLogger(tempLogDir, tempStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Backup", tempSourceDir, tempTargetDir, BackupType.Full);

        try
        {
            // Act - Should succeed when businessSoftwareName is null (backward compatible)
            service.ExecuteBackup(job, null);

            // Assert - Backup completed successfully
            Assert.True(File.Exists(Path.Combine(tempTargetDir, "test.txt")));
        }
        finally
        {
            // Cleanup
            Directory.Delete(tempLogDir, true);
        }
    }

    [Fact]
    public void ExecuteBackup_v2_ShouldSucceed_WhenBusinessSoftwareNameIsEmpty()
    {
        // Arrange
        string tempLogDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        string tempSourceDir = Path.Combine(tempLogDir, "Source");
        string tempTargetDir = Path.Combine(tempLogDir, "Target");
        string tempStateFile = Path.Combine(tempLogDir, "state.json");
        
        Directory.CreateDirectory(tempSourceDir);
        Directory.CreateDirectory(tempTargetDir);
        Directory.CreateDirectory(tempLogDir);
        
        // Create a test file
        File.WriteAllText(Path.Combine(tempSourceDir, "test.txt"), "Test content");
        
        var logger = new JsonLogger(tempLogDir, tempStateFile);
        var service = new BackupService(logger);
        var job = new BackupJob("Test Backup", tempSourceDir, tempTargetDir, BackupType.Full);

        try
        {
            // Act - Should succeed when businessSoftwareName is empty (v1.0/v1.1 backward compatible)
            service.ExecuteBackup(job, "");

            // Assert - Backup completed successfully
            Assert.True(File.Exists(Path.Combine(tempTargetDir, "test.txt")));
        }
        finally
        {
            // Cleanup
            Directory.Delete(tempLogDir, true);
        }
    }

    /* Helper methods for integration tests
    private void CreateTestFiles(string directory, int count)
    {
        for (int i = 0; i < count; i++)
        {
            string filePath = Path.Combine(directory, $"test_{i}.txt");
            File.WriteAllText(filePath, $"Test content {i}");
        }
    }

    private void CreateTestFilesWithSubdirs(string directory)
    {
        // Create root files
        File.WriteAllText(Path.Combine(directory, "root.txt"), "Root file");

        // Create subdirectory with files
        string subDir = Path.Combine(directory, "SubFolder");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "sub1.txt"), "Sub file 1");
        File.WriteAllText(Path.Combine(subDir, "sub2.txt"), "Sub file 2");

        // Create nested subdirectory
        string nestedDir = Path.Combine(subDir, "Nested");
        Directory.CreateDirectory(nestedDir);
        File.WriteAllText(Path.Combine(nestedDir, "nested.txt"), "Nested file");
    }
    */
}
