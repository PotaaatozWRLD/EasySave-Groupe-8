using System.Text.Json;
using EasySave.Shared;

namespace EasyLog;

/// <summary>
/// Interface for logging backup operations and state updates.
/// Part of EasyLog.dll - must remain compatible across versions and platforms.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Writes a log entry for a file operation.
    /// </summary>
    void WriteLog(LogEntry logEntry);
    
    /// <summary>
    /// Updates the real-time state of a backup job.
    /// </summary>
    void UpdateState(StateEntry stateEntry);
}

/// <summary>
/// JSON implementation of the ILogger interface.
/// Writes daily log files and a real-time state file.
/// </summary>
public class JsonLogger : ILogger
{
    private readonly string _logDirectory;
    private readonly string _stateFilePath;

    public JsonLogger(string logDirectory, string stateFilePath)
    {
        _logDirectory = logDirectory;
        _stateFilePath = stateFilePath;

        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    public void WriteLog(LogEntry logEntry)
    {
        string logFilePath = Path.Combine(_logDirectory, $"{DateTime.Now:yyyy-MM-dd}.json");
        var options = new JsonSerializerOptions { WriteIndented = true };
        var logContent = JsonSerializer.Serialize(logEntry, options);
        File.AppendAllText(logFilePath, logContent + Environment.NewLine);
    }

    public void UpdateState(StateEntry stateEntry)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var stateContent = JsonSerializer.Serialize(stateEntry, options);
        File.WriteAllText(_stateFilePath, stateContent);
    }
}