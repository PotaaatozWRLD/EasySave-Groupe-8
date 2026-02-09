using EasyLog;
using Xunit;
using System.Xml.Linq;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for XmlLogger class.
/// Tests XML logging functionality, indentation, and backward compatibility.
/// </summary>
public class XmlLoggerTests : IDisposable
{
    private readonly string _testLogPath;
    private readonly string _testStatePath;
    private readonly string _testDir;

    public XmlLoggerTests()
    {
        // Create test directories
        _testDir = Path.Combine(Path.GetTempPath(), "EasySaveTests", Guid.NewGuid().ToString());
        _testLogPath = Path.Combine(_testDir, "Logs");
        _testStatePath = Path.Combine(_testDir, "state.json");
        
        // Create both parent directory and logs subdirectory
        Directory.CreateDirectory(_testDir);
        Directory.CreateDirectory(_testLogPath);
    }

    [Fact]
    public void XmlLogger_ShouldCreateDailyLogFile()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
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
        string expectedFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.xml");
        Assert.True(File.Exists(expectedFile), $"XML log file should exist at {expectedFile}");
    }

    [Fact]
    public void XmlLogger_ShouldCreateStateFile()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var state = new StateEntry
        {
            Name = "Test Job",
            State = JobState.ACTIVE,
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalFiles = 10,
            TotalSize = 1024000,
            Progression = 50,
            NbFilesLeftToDo = 5,
            NbFilesLeftToDoSize = 512000,
            CurrentSourceFilePath = "C:\\source\\file.txt",
            CurrentTargetFilePath = "D:\\target\\file.txt"
        };

        // Act
        var exception = Record.Exception(() => logger.UpdateState(state));
        
        // Assert no exception
        Assert.Null(exception);

        // Assert file exists
        string expectedFile = Path.ChangeExtension(_testStatePath, ".xml");
        Assert.True(File.Exists(expectedFile), $"State XML file should exist at {expectedFile}. Test dir exists: {Directory.Exists(_testDir)}. Files in test dir: {string.Join(", ", Directory.GetFiles(_testDir))}");
        
        // Verify content
        XDocument doc = XDocument.Load(expectedFile);
        Assert.NotNull(doc.Root);
        Assert.Equal("StateEntries", doc.Root.Name.LocalName);
    }

    [Fact]
    public void XmlLogger_ShouldCreateValidXmlStructure()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var entry = new LogEntry
        {
            JobName = "XML Test",
            SourcePath = "\\\\server\\source\\file.txt",
            TargetPath = "\\\\server\\target\\file.txt",
            FileName = "file.txt",
            FileSize = 2048,
            TransferTime = 150,
            Timestamp = DateTime.Now
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.xml");
        XDocument doc = XDocument.Load(logFile);
        
        Assert.NotNull(doc.Root);
        Assert.Equal("LogEntries", doc.Root.Name.LocalName);
        Assert.Single(doc.Root.Elements("LogEntry"));
        
        var logEntry = doc.Root.Element("LogEntry");
        Assert.NotNull(logEntry);
        Assert.Equal("XML Test", logEntry.Element("JobName")?.Value);
        Assert.Equal("file.txt", logEntry.Element("FileName")?.Value);
        Assert.Equal("2048", logEntry.Element("FileSize")?.Value);
    }

    [Fact]
    public void XmlLogger_ShouldBeIndentedForNotepadReadability()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var entry = new LogEntry
        {
            JobName = "Indentation Test",
            SourcePath = "C:\\source",
            TargetPath = "D:\\target",
            FileName = "test.txt",
            FileSize = 100,
            TransferTime = 10,
            Timestamp = DateTime.Now
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.xml");
        string content = File.ReadAllText(logFile);
        
        // Check for indentation (should have newlines and spaces)
        Assert.Contains("\n", content);
        Assert.Contains("  <LogEntry>", content);
        Assert.Contains("    <JobName>", content);
    }

    [Fact]
    public void XmlLogger_ShouldAppendMultipleEntries()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var entry1 = new LogEntry
        {
            JobName = "Job 1",
            SourcePath = "C:\\file1.txt",
            TargetPath = "D:\\file1.txt",
            FileName = "file1.txt",
            FileSize = 100,
            TransferTime = 10,
            Timestamp = DateTime.Now
        };
        var entry2 = new LogEntry
        {
            JobName = "Job 2",
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
        string logFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.xml");
        XDocument doc = XDocument.Load(logFile);
        
        Assert.Equal(2, doc.Root?.Elements("LogEntry").Count());
    }

    [Fact]
    public void XmlLogger_ShouldHandleErrorMessages()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var entry = new LogEntry
        {
            JobName = "Failed Job",
            SourcePath = "C:\\missing\\file.txt",
            TargetPath = "D:\\target\\file.txt",
            FileName = "file.txt",
            FileSize = 0,
            TransferTime = -1,
            ErrorMessage = "File not found",
            Timestamp = DateTime.Now
        };

        // Act
        logger.WriteLog(entry);

        // Assert
        string logFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.xml");
        XDocument doc = XDocument.Load(logFile);
        
        var logEntry = doc.Root?.Element("LogEntry");
        Assert.NotNull(logEntry);
        Assert.Equal("-1", logEntry.Element("TransferTime")?.Value);
        Assert.Equal("File not found", logEntry.Element("ErrorMessage")?.Value);
    }

    [Fact]
    public void XmlLogger_ShouldHandleCorruptedFiles()
    {
        // Arrange
        string corruptedFile = Path.Combine(_testLogPath, $"{DateTime.Now:yyyy-MM-dd}.xml");
        File.WriteAllText(corruptedFile, "This is not valid XML");
        
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var entry = new LogEntry
        {
            JobName = "Recovery Test",
            SourcePath = "C:\\test.txt",
            TargetPath = "D:\\test.txt",
            FileName = "test.txt",
            FileSize = 100,
            TransferTime = 10,
            Timestamp = DateTime.Now
        };

        // Act & Assert (should not throw exception)
        var exception = Record.Exception(() => logger.WriteLog(entry));
        Assert.Null(exception);
        
        // File should be recreated with valid XML
        XDocument doc = XDocument.Load(corruptedFile);
        Assert.NotNull(doc.Root);
    }

    [Fact]
    public void XmlLogger_ShouldUpdateStateCorrectly()
    {
        // Arrange
        var logger = new XmlLogger(_testLogPath, _testStatePath);
        var state1 = new StateEntry
        {
            Name = "Job A",
            State = JobState.ACTIVE,
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalFiles = 10,
            Progression = 30
        };
        var state2 = new StateEntry
        {
            Name = "Job A",
            State = JobState.ACTIVE,
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalFiles = 10,
            Progression = 60
        };

        // Act
        logger.UpdateState(state1);
        logger.UpdateState(state2);

        // Assert
        string stateFile = Path.ChangeExtension(_testStatePath, ".xml");
        XDocument doc = XDocument.Load(stateFile);
        
        // Should only have one entry (updated, not duplicated)
        Assert.Single(doc.Root?.Elements("StateEntry"));
        
        var stateEntry = doc.Root?.Element("StateEntry");
        Assert.NotNull(stateEntry);
        Assert.Equal("60", stateEntry.Element("Progression")?.Value);
    }

    [Fact]
    public void XmlLogger_ShouldImplementILoggerInterface()
    {
        // Arrange & Act
        ILogger logger = new XmlLogger(_testLogPath, _testStatePath);

        // Assert
        Assert.NotNull(logger);
        Assert.IsAssignableFrom<ILogger>(logger);
    }

    [Fact]
    public void LoggerFactory_ShouldCreateXmlLogger()
    {
        // Arrange & Act
        ILogger logger = LoggerFactory.CreateLogger(LogFormat.XML, _testLogPath, _testStatePath);

        // Assert
        Assert.NotNull(logger);
        Assert.IsType<XmlLogger>(logger);
    }

    [Fact]
    public void LoggerFactory_ShouldCreateJsonLogger()
    {
        // Arrange & Act
        ILogger logger = LoggerFactory.CreateLogger(LogFormat.JSON, _testLogPath, _testStatePath);

        // Assert
        Assert.NotNull(logger);
        Assert.IsType<JsonLogger>(logger);
    }

    public void Dispose()
    {
        // Cleanup test directory
        if (Directory.Exists(_testDir))
        {
            try
            {
                Directory.Delete(_testDir, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
