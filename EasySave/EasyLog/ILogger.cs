using System.Text.Json;
using EasySave.Shared;

namespace EasyLog;

public interface ILogger
{
    void WriteLog(LogEntry logEntry);
    void UpdateState(StateEntry stateEntry);
}

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