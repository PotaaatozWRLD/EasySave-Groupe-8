namespace EasySave.Core.Models;

/// <summary>
/// Defines the type of backup operation.
/// </summary>
public enum BackupType
{
    Full,          // Complete backup of all files
    Differential   // Only files modified since last backup
}

/// <summary>
/// Represents a backup job configuration.
/// Maximum of 5 jobs allowed per user.
/// </summary>
public class BackupJob
{
    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public BackupType Type { get; set; }

    public BackupJob(string name, string sourcePath, string targetPath, BackupType type)
    {
        Name = name;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        Type = type;
    }
}