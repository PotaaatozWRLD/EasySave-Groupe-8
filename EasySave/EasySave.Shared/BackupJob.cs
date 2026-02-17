using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace EasySave.Shared;

public enum BackupType
{
    Full,
    Differential
}

public class BackupJob : INotifyPropertyChanged
{
    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public BackupType Type { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastExecutionDate { get; set; }
    public string? Description { get; set; }

    // Runtime properties (not serialized)
    private int _progress = 0;
    [JsonIgnore]
    public int Progress
    {
        get => _progress;
        set
        {
            if (_progress != value)
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
    }

    private string _state = "Idle";
    [JsonIgnore]
    public string State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                OnPropertyChanged();
                // Notify dependent properties
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(CanResume));
                OnPropertyChanged(nameof(CanStop));
                OnPropertyChanged(nameof(IsRunning));
            }
        }
    }

    [JsonIgnore]
    public bool CanPause => State == "Active";
    
    [JsonIgnore]
    public bool CanResume => State == "Paused";

    [JsonIgnore]
    public bool CanStop => State == "Active" || State == "Paused";

    [JsonIgnore] 
    public bool IsRunning => State == "Active" || State == "Paused";

    public BackupJob(string name, string sourcePath, string targetPath, BackupType type)
    {
        Name = name;
        SourcePath = sourcePath;
        TargetPath = targetPath;
        Type = type;
        CreatedDate = DateTime.Now;
        Description = string.Empty;
        State = "Idle";
        Progress = 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}