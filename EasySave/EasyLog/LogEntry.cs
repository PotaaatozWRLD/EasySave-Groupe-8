namespace EasyLog;

public class LogEntry
{
    public string JobName { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public string TargetPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public long TransferTime { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
}