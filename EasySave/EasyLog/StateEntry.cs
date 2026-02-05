namespace EasyLog;

/// <summary>
/// Represents the current state of a backup job.
/// </summary>
public enum JobState
{
    ACTIVE,  // Backup in progress
    END,     // Backup completed
    PAUSED   // Backup paused
}

/// <summary>
/// Real-time state entry for a backup job.
/// Written to state.json and updated during backup execution.
/// </summary>
public class StateEntry
{
    public string Name { get; set; } = string.Empty;
    public string LastActionTimestamp { get; set; } = string.Empty;
    public JobState State { get; set; }
    public int TotalFiles { get; set; }                         // Total files to backup
    public long TotalSize { get; set; }                         // Total size in bytes
    public int Progression { get; set; }                        // Progress percentage (0-100)
    public int NbFilesLeftToDo { get; set; }                   // Remaining files
    public long NbFilesLeftToDoSize { get; set; }              // Remaining size in bytes
    public string CurrentSourceFilePath { get; set; } = string.Empty;
    public string CurrentTargetFilePath { get; set; } = string.Empty;
}