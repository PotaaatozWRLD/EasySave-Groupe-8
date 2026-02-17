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
    public long EncryptionTime { get; set; } = 0;            // Time in ms (0=no encryption, >0=success, <0=error) - v2.0
    public string? ErrorMessage { get; set; }                // Null if no error
    public DateTime Timestamp { get; set; } = DateTime.Now; // Auto-set to current time

    // V3.0: Centralized Logging Identity
    public string MachineName { get; set; } = Environment.MachineName;
    public string UserName { get; set; } = Environment.UserName;
}