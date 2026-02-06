using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for JsonLogger class.
/// Tests logging functionality and JSON file creation.
/// </summary>
public class JsonLoggerTests
{
    private readonly string _testLogPath;
    private readonly string _testStatePath;

    public JsonLoggerTests()
    {
        // Create test directories
        string testDir = Path.Combine(Path.GetTempPath(), "EasySaveTests", Guid.NewGuid().ToString());
        _testLogPath = Path.Combine(testDir, "Logs");
        _testStatePath = Path.Combine(testDir, "state.json");
        Directory.CreateDirectory(_testLogPath);
    }

    [Fact]
    public void JsonLogger_ShouldCreateDailyLogFile()
    {
        // Arrange
        var logger = new JsonLogger(_testLogPath, _testStatePath);
        var entry = new LogEntry
        {
            JobName = "Test Job",
            SourcePath = "C:\\source\\file.txt",
            TargetPath = "D:\\target\\file.txt",
            FileName = "file.txt",
            FileSize = 1024,
            TransferTime = 100,
            Timestamp = DateTime.Now
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string expectedFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.json");
        Assert.True(File.Exists(expectedFile), $"Log file should exist at {expectedFile}");
    }

    [Fact]
    public void JsonLogger_ShouldCreateStateFile()
    {
        // Arrange
        var logger = new JsonLogger(_testLogPath, _testStatePath);
        var state = new StateEntry
        {
            Name = "Test Job",
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            State = JobState.ACTIVE,
            TotalFiles = 10,
            TotalSize = 10240,
            Progression = 50,
            NbFilesLeftToDo = 5,
            NbFilesLeftToDoSize = 5120,
            CurrentSourceFilePath = "C:\\source\\current.txt",
            CurrentTargetFilePath = "D:\\target\\current.txt"
        };

        // Act
        logger.UpdateState(state);

        // Assert
        Assert.True(File.Exists(_testStatePath), $"State file should exist at {_testStatePath}");
    }

    [Fact]
    public void JsonLogger_ShouldWriteValidJsonFormat()
    {
        // Arrange
        var logger = new JsonLogger(_testLogPath, _testStatePath);
        var entry = new LogEntry
        {
            JobName = "JSON Test",
            SourcePath = "C:\\test.txt",
            TargetPath = "D:\\test.txt",
            FileName = "test.txt",
            FileSize = 512,
            TransferTime = 50,
            Timestamp = DateTime.Now
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.json");
        string content = File.ReadAllText(logFile);
        
        // Should be valid JSON (indented)
        Assert.Contains("{", content);
        Assert.Contains("}", content);
        Assert.Contains("\"JobName\"", content);
    }

    [Fact]
    public void JsonLogger_ShouldAppendToExistingDailyLog()
    {
        // Arrange
        var logger = new JsonLogger(_testLogPath, _testStatePath);
        var entry1 = new LogEntry
        {
            JobName = "Entry 1",
            SourcePath = "C:\\file1.txt",
            TargetPath = "D:\\file1.txt",
            FileName = "file1.txt",
            FileSize = 100,
            TransferTime = 10,
            Timestamp = DateTime.Now
        };
        var entry2 = new LogEntry
        {
            JobName = "Entry 2",
            SourcePath = "C:\\file2.txt",
            TargetPath = "D:\\file2.txt",
            FileName = "file2.txt",
            FileSize = 200,
            TransferTime = 20,
            Timestamp = DateTime.Now
        };

        // Act
        logger.WriteLog(entry1);
        logger.WriteLog(entry2);

        // Assert
        string logFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.json");
        string content = File.ReadAllText(logFile);
        Assert.Contains("Entry 1", content);
        Assert.Contains("Entry 2", content);
    }
}
