namespace EasyLog;

/// <summary>
/// Represents a log entry for a file transfer operation.
/// Written to daily JSON log files.
/// </summary>
public class LogEntry
{
    public string JobName { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;  // Full UNC path
    public string TargetPath { get; set; } = string.Empty;  // Full UNC path
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }                       // Size in bytes
    public long TransferTime { get; set; }                   // Time in ms (negative if error)
    public string? ErrorMessage { get; set; }                // Null if no error
    public DateTime Timestamp { get; set; } = DateTime.Now; // Auto-set to current time
}