using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Shared;
using EasySave.GUI.Helpers;

namespace EasySave.GUI.ViewModels;

/// <summary>
/// ViewModel for creating or editing a backup job.
/// </summary>
public partial class JobEditorViewModel : ViewModelBase
{
    public LocalizationManager Localization => LocalizationManager.Instance;
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _targetPath = string.Empty;

    [ObservableProperty]
    private BackupType _selectedBackupType = BackupType.Full;

    [ObservableProperty]
    private string? _validationError;

    public ObservableCollection<BackupType> BackupTypes { get; }
    
    public bool IsEditMode { get; }
    public bool DialogResult { get; private set; }
    
    public event EventHandler? CloseRequested;

    /// <summary>
    /// Constructor for creating a new job.
    /// </summary>
    public JobEditorViewModel()
    {
        BackupTypes = new ObservableCollection<BackupType>(Enum.GetValues<BackupType>());
        IsEditMode = false;
    }

    /// <summary>
    /// Constructor for editing an existing job.
    /// </summary>
    public JobEditorViewModel(BackupJob job) : this()
    {
        Name = job.Name;
        SourcePath = job.SourcePath;
        TargetPath = job.TargetPath;
        SelectedBackupType = job.Type;
        IsEditMode = true;
    }

    [RelayCommand]
    private void Save()
    {
        if (ValidateInput())
        {
            DialogResult = true;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task BrowseSource()
    {
        var dialog = new Avalonia.Platform.Storage.FolderPickerOpenOptions
        {
            Title = "Select Source Folder",
            AllowMultiple = false
        };

        var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (topLevel != null)
        {
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(dialog);
            if (folders.Count > 0)
            {
                SourcePath = folders[0].Path.LocalPath;
            }
            
            // Bring window back to focus
            topLevel.Activate();
        }
    }

    [RelayCommand]
    private async Task BrowseTarget()
    {
        var dialog = new Avalonia.Platform.Storage.FolderPickerOpenOptions
        {
            Title = "Select Target Folder",
            AllowMultiple = false
        };

        var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (topLevel != null)
        {
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(dialog);
            if (folders.Count > 0)
            {
                TargetPath = folders[0].Path.LocalPath;
            }
            
            // Bring window back to focus
            topLevel.Activate();
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ValidationError = "Job name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SourcePath))
        {
            ValidationError = "Source path is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(TargetPath))
        {
            ValidationError = "Target path is required.";
            return false;
        }

        if (!Directory.Exists(SourcePath) && !SourcePath.StartsWith(@"\\"))
        {
            ValidationError = "Source path does not exist.";
            return false;
        }

        ValidationError = null;
        return true;
    }

    /// <summary>
    /// Creates a BackupJob from the current input values.
    /// </summary>
    public BackupJob CreateJob()
    {
        return new BackupJob(Name, SourcePath, TargetPath, SelectedBackupType);
    }
}
