namespace EasyLog;

public enum JobState
{
    ACTIVE,
    END,
    PAUSED
}

public class StateEntry
{
    public string Name { get; set; } = string.Empty;
    public string LastActionTimestamp { get; set; } = string.Empty;
    public JobState State { get; set; }
    public int TotalFiles { get; set; }
    public long TotalSize { get; set; }
    public int Progression { get; set; }
    public int NbFilesLeftToDo { get; set; }
    public long NbFilesLeftToDoSize { get; set; }
    public string CurrentSourceFilePath { get; set; } = string.Empty;
    public string CurrentTargetFilePath { get; set; } = string.Empty;
}