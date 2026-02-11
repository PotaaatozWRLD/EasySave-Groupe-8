namespace EasySave.Shared;

public enum BackupType
{
    Full,
    Differential
}

public class BackupJob
{
    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public BackupType Type { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastExecutionDate { get; set; }
    public string? Description { get; set; }

    public BackupJob(string name, string sourcePath, string targetPath, BackupType type)
    {
        Name = name;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        Type = type;
        CreatedDate = DateTime.Now;
        Description = string.Empty;
    }
}