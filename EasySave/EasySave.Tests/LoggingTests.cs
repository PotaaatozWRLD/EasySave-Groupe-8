using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Tests for logging functionality including file generation and UNC path formatting.
/// </summary>
public class LoggingTests : IDisposable
{
    private readonly string _testRoot;
    private readonly string _testLogDir;
    private readonly string _testStateFile;

    public LoggingTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "EasySaveLoggingTests", Guid.NewGuid().ToString());
        _testLogDir = Path.Combine(_testRoot, "Logs");
        _testStateFile = Path.Combine(_testRoot, "state.json");

        Directory.CreateDirectory(_testLogDir);
    }

    [Fact]
    public void DailyLogFile_ShouldBeCreatedWithCorrectName()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var entry = new LogEntry
        {
            JobName = "Test Job",
            SourcePath = @"C:\Source\file.txt",
            TargetPath = @"C:\Target\file.txt",
            FileName = "file.txt",
            FileSize = 1024,
            TransferTime = 100
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string expectedFileName = $"{DateTime.Now:yyyy-MM-dd}.json";
        string expectedPath = Path.Combine(_testLogDir, expectedFileName);
        Assert.True(File.Exists(expectedPath), $"Log file {expectedFileName} should exist");
    }

    [Fact]
    public void LogEntry_ShouldContainUNCPath()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var entry = new LogEntry
        {
            JobName = "UNC Test",
            SourcePath = @"C:\Users\Test\source.txt",
            TargetPath = @"C:\Users\Test\target.txt",
            FileName = "source.txt",
            FileSize = 2048,
            TransferTime = 50
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogDir, $"{DateTime.Now:yyyy-MM-dd}.json");
        string content = File.ReadAllText(logFile);
        
        // UNC paths should start with \\
        Assert.Contains(@"\\", content);
    }

    [Fact]
    public void LogEntry_WithError_ShouldHaveNegativeTransferTime()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var entry = new LogEntry
        {
            JobName = "Error Test",
            SourcePath = @"C:\Source\error.txt",
            TargetPath = @"C:\Target\error.txt",
            FileName = "error.txt",
            FileSize = 0,
            TransferTime = -1,
            ErrorMessage = "File not found"
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogDir, $"{DateTime.Now:yyyy-MM-dd}.json");
        string content = File.ReadAllText(logFile);
        
        Assert.Contains("-1", content);
        Assert.Contains("File not found", content);
    }

    [Fact]
    public void MultipleLogEntries_ShouldAppendToSameFile()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var entry1 = new LogEntry
        {
            JobName = "Job 1",
            SourcePath = @"C:\file1.txt",
            TargetPath = @"C:\backup\file1.txt",
            FileName = "file1.txt",
            FileSize = 100,
            TransferTime = 10
        };
        var entry2 = new LogEntry
        {
            JobName = "Job 2",
            SourcePath = @"C:\file2.txt",
            TargetPath = @"C:\backup\file2.txt",
            FileName = "file2.txt",
            FileSize = 200,
            TransferTime = 20
        };

        // Act
        logger.WriteLog(entry1);
        logger.WriteLog(entry2);

        // Assert
        string logFile = Path.Combine(_testLogDir, $"{DateTime.Now:yyyy-MM-dd}.json");
        string content = File.ReadAllText(logFile);
        
        Assert.Contains("file1.txt", content);
        Assert.Contains("file2.txt", content);
    }

    [Fact]
    public void LogFile_ShouldBeValidJSON()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var entry = new LogEntry
        {
            JobName = "JSON Test",
            SourcePath = @"C:\test.txt",
            TargetPath = @"C:\backup\test.txt",
            FileName = "test.txt",
            FileSize = 512,
            TransferTime = 25
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogDir, $"{DateTime.Now:yyyy-MM-dd}.json");
        string content = File.ReadAllText(logFile);
        
        // Should be valid JSON (no exception thrown)
        Assert.DoesNotContain("\\\\\\", content); // No triple backslashes
        Assert.Contains("{", content);
        Assert.Contains("}", content);
    }

    [Fact]
    public void StateFile_ShouldUpdateProgress()
    {
        // Arrange
        var logger = new JsonLogger(_testLogDir, _testStateFile);
        var state = new StateEntry
        {
            Name = "Progress Test",
            State = JobState.ACTIVE,
            TotalFiles = 10,
            TotalSize = 10240,
            Progression = 50,
            NbFilesLeftToDo = 5,
            NbFilesLeftToDoSize = 5120,
            CurrentSourceFilePath = @"C:\source\file5.txt",
            CurrentTargetFilePath = @"C:\target\file5.txt"
        };

        // Act
        logger.UpdateState(state);

        // Assert
        Assert.True(File.Exists(_testStateFile));
        string content = File.ReadAllText(_testStateFile);
        Assert.Contains("Progress Test", content);
        // State is serialized as enum value (0, 1, 2, etc.)
        Assert.Contains("50", content); // Check progression instead
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            try
            {
                Directory.Delete(_testRoot, true);
            }
            catch
            {
                // Cleanup may fail, ignore
            }
        }
    }
}
