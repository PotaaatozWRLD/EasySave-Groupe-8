using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Core.Services;
using EasySave.Shared;
using EasySave.GUI.Helpers;
using EasySave.ConsoleApp.Config;
using EasyLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasySave.GUI.ViewModels;

/// <summary>
/// Main ViewModel for EasySave application.
/// Manages backup jobs, execution, and application state.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    public LocalizationManager Localization => LocalizationManager.Instance;
    private readonly string _appDataPath;
    private readonly string _logPath;
    private readonly string _statePath;
    
    private ILogger? _logger;

    [ObservableProperty]
    private ObservableCollection<BackupJob> _jobs = new();

    [ObservableProperty]
    private ObservableCollection<BackupJob> _selectedJobs = new();

    [ObservableProperty]
    private BackupJob? _selectedJob;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isExecuting = false;

    public MainViewModel()
    {

        // Initialize paths (same as Console app)
        _appDataPath = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProSoft", "EasySave");
        _logPath = Path.Join(_appDataPath, "Logs");
        _statePath = Path.Join(_appDataPath, "state.json");

        Directory.CreateDirectory(_appDataPath);
        Directory.CreateDirectory(_logPath);

        InitializeServices();
        LoadJobs();
    }

    private void InitializeServices()
    {
        // Initialize logger with current configuration
        LogFormat logFormat = AppConfig.GetLogFormat();
        _logger = LoggerFactory.CreateLogger(logFormat, _logPath, _statePath);
    }

    private void LoadJobs()
    {
        Jobs.Clear();
        JobManager.LoadJobs();
        foreach (var job in JobManager.Jobs)
        {
            Jobs.Add(job);
        }

        StatusMessage = $"{Jobs.Count} jobs loaded";
    }

    [RelayCommand]
    private void CreateJob()
    {
        var editorViewModel = new JobEditorViewModel();
        var editorWindow = new Views.JobEditorWindow(editorViewModel);
        
        editorViewModel.CloseRequested += (s, e) =>
        {
            if (editorViewModel.DialogResult)
            {
                var newJob = editorViewModel.CreateJob();
                JobManager.AddJob(newJob.Name, newJob.SourcePath, newJob.TargetPath, newJob.Type);
                Jobs.Add(newJob);
                StatusMessage = "Job created successfully";
            }
        };
        
        editorWindow.Show();
    }

    [RelayCommand]
    private void EditJob()
    {
        if (SelectedJob == null)
        {
            StatusMessage = "No job selected";
            return;
        }

        var editorViewModel = new JobEditorViewModel(SelectedJob);
        var editorWindow = new Views.JobEditorWindow(editorViewModel);
        
        editorViewModel.CloseRequested += (s, e) =>
        {
            if (editorViewModel.DialogResult)
            {
                var updatedJob = editorViewModel.CreateJob();
                
                var index = Jobs.IndexOf(SelectedJob);
                if (index >= 0)
                {
                    JobManager.DeleteJob(index);
                    JobManager.AddJob(updatedJob.Name, updatedJob.SourcePath, updatedJob.TargetPath, updatedJob.Type);
                    Jobs[index] = updatedJob;
                    SelectedJob = updatedJob;
                }
                
                StatusMessage = $"Job '{updatedJob.Name}' updated";
            }
        };
        
        editorWindow.Show();
    }

    [RelayCommand]
    private void DeleteJob()
    {
        if (SelectedJob == null)
        {
            StatusMessage = "No job selected";
            return;
        }

        try
        {
            int index = Jobs.IndexOf(SelectedJob);
            JobManager.DeleteJob(index);
            Jobs.Remove(SelectedJob);
            StatusMessage = $"Job '{SelectedJob.Name}' deleted";
            SelectedJob = null;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            StatusMessage = $"Error deleting job: {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = $"Error deleting job: {ex.Message}";
        }
        catch (IOException ex)
        {
            StatusMessage = $"Error deleting job: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ExecuteSelectedJobAsync()
    {
        // Execute selected jobs (support multiple selection)
        if (SelectedJobs.Count == 0 || _logger == null)
        {
            StatusMessage = "No job selected";
            return;
        }

        IsExecuting = true;
        int totalJobs = SelectedJobs.Count;
        int successCount = 0;
        int failCount = 0;

        StatusMessage = $"Executing {totalJobs} job(s)...";

        try
        {
            // Create backup service with encryption support
            string cryptoSoftPath = AppConfig.GetCryptoSoftPath();
            List<string> extensionsToEncrypt = AppConfig.GetExtensionsToEncrypt();
            var backupService = new BackupService(_logger, cryptoSoftPath, extensionsToEncrypt);
            string? businessSoftware = AppConfig.GetBusinessSoftwareName();
            
            foreach (var job in SelectedJobs.ToList())
            {
                try
                {
                    StatusMessage = $"Executing: {job.Name} ({successCount + failCount + 1}/{totalJobs})...";
                    await Task.Run(() => backupService.ExecuteBackup(job, businessSoftware));
                    successCount++;
                }
                catch (IOException ex)
                {
                    failCount++;
                    StatusMessage = $"Failed: {job.Name} - I/O error: {ex.Message}";
                    await Task.Delay(2000); // Show error for 2 seconds
                }
                catch (UnauthorizedAccessException ex)
                {
                    failCount++;
                    StatusMessage = $"Failed: {job.Name} - Access denied: {ex.Message}";
                    await Task.Delay(2000); // Show error for 2 seconds
                }
                catch (Exception ex) when (ex is not OutOfMemoryException && ex is not StackOverflowException)
                {
                    // Catch all backup-related exceptions (network, file system, etc.)
                    failCount++;
                    StatusMessage = $"Failed: {job.Name} - {ex.Message}";
                    await Task.Delay(2000); // Show error for 2 seconds
                }
            }
            
            StatusMessage = $"Completed: {successCount} succeeded, {failCount} failed";
        }
        catch (IOException ex)
        {
            StatusMessage = $"Configuration or I/O error: {ex.Message}";
        }
        catch (Exception ex) when (ex is not OutOfMemoryException && ex is not StackOverflowException)
        {
            // Unexpected error during job preparation or service initialization
            StatusMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsExecuting = false;
        }
    }

    [RelayCommand]
    private async Task ExecuteAllJobsAsync()
    {
        if (Jobs.Count == 0 || _logger == null)
        {
            StatusMessage = "No jobs to execute";
            return;
        }

        IsExecuting = true;
        StatusMessage = "Executing all jobs...";

        try
        {
            // Create backup service with encryption support
            string cryptoSoftPath = AppConfig.GetCryptoSoftPath();
            List<string> extensionsToEncrypt = AppConfig.GetExtensionsToEncrypt();
            var backupService = new BackupService(_logger, cryptoSoftPath, extensionsToEncrypt);
            string? businessSoftware = AppConfig.GetBusinessSoftwareName();
            
            foreach (var job in Jobs)
            {
                StatusMessage = $"Executing: {job.Name}...";
                await Task.Run(() => backupService.ExecuteBackup(job, businessSoftware));
            }

            StatusMessage = $"All {Jobs.Count} jobs completed successfully";
        }
        catch (IOException ex)
        {
            StatusMessage = $"I/O error: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            StatusMessage = $"Access denied: {ex.Message}";
        }
        catch (Exception ex) when (ex is not OutOfMemoryException && ex is not StackOverflowException)
        {
            // Catch unexpected errors during backup execution
            StatusMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsExecuting = false;
        }
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var settingsViewModel = new SettingsViewModel();
        var settingsWindow = new Views.SettingsWindow(settingsViewModel);
        
        settingsViewModel.CloseRequested += (s, e) =>
        {
            if (settingsViewModel.DialogResult)
            {
                InitializeServices();
                StatusMessage = "Settings saved successfully";
            }
        };
        
        settingsWindow.Show();
    }

    [RelayCommand]
    private void SwitchLanguage()
    {
        LocalizationManager.Instance.ToggleLanguage();
        StatusMessage = $"Language: {LocalizationManager.Instance.CurrentCulture.ToUpper()}";
    }

    [RelayCommand]
    private void Exit()
    {
        System.Environment.Exit(0);
    }
}
