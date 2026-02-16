using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.Core.Services;
using EasySave.Shared;
using EasySave.GUI.Helpers;
using EasySave.ConsoleApp.Config;
using EasySave.ConsoleApp.Languages;
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
    [RelayCommand]
    private async Task DecryptFileAsync()
    {
        try
        {
            // Ouvre une boîte de dialogue pour sélectionner le fichier à décrypter (API moderne)
            var window = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
            if (window == null)
            {
                StatusMessage = LanguageManager.Instance.GetString("Error_WindowNotFound");
                return;
            }
            
            var files = await window.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = LanguageManager.Instance.GetString("Decrypt_Title"),
                AllowMultiple = true
            });
            
            if (files.Count == 0)
            {
                StatusMessage = LanguageManager.Instance.GetString("Decrypt_NoFileSelected");
                return;
            }
            
            string cryptoSoftPath = AppConfig.GetCryptoSoftPath();
            if (!File.Exists(cryptoSoftPath))
            {
                StatusMessage = LanguageManager.Instance.GetString("Decrypt_CryptoSoftNotFound");
                return;
            }

            int successCount = 0;
            int errorCount = 0;

            foreach (var file in files)
            {
                try
                {
                    string inputFile = file.Path.LocalPath;
                    string tempFile = inputFile + ".tmp";

                    // Appel CryptoSoft.exe pour décrypter (XOR roundtrip - le XOR est réversible)
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = cryptoSoftPath;
                    process.StartInfo.Arguments = $"\"{inputFile}\" \"{tempFile}\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();
                    await Task.Run(() => process.WaitForExit());
                    
                    if (process.ExitCode == 0 && File.Exists(tempFile))
                    {
                        File.Delete(inputFile);
                        File.Move(tempFile, inputFile);
                        successCount++;
                    }
                    else
                    {
                        if (File.Exists(tempFile)) File.Delete(tempFile);
                        errorCount++;
                    }
                }
                catch
                {
                    errorCount++;
                }
            }

            if (errorCount == 0)
                StatusMessage = string.Format(LanguageManager.Instance.GetString("Decrypt_Success"), successCount);
            else
                StatusMessage = string.Format(LanguageManager.Instance.GetString("Decrypt_Error"), successCount, errorCount);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format(LanguageManager.Instance.GetString("Decrypt_ErrorGeneric"), ex.Message);
        }
    }

    [RelayCommand]
    private void OpenLogsFolder()
    {
        try
        {
            var logsPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProSoft", "EasySave", "Logs");
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);

            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = logsPath,
                    UseShellExecute = true
                });
            }
            else if (OperatingSystem.IsLinux())
            {
                System.Diagnostics.Process.Start("xdg-open", logsPath);
            }
            else if (OperatingSystem.IsMacOS())
            {
                System.Diagnostics.Process.Start("open", logsPath);
            }
            
            StatusMessage = LanguageManager.Instance.GetString("Logs_Opened");
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format(LanguageManager.Instance.GetString("Logs_Error"), ex.Message);
        }
    }
//
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

    [ObservableProperty]
    private double _progressPercentage = 0;

    [ObservableProperty]
    private string _progressText = string.Empty;

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
                StatusMessage = LanguageManager.Instance.GetString("Job_Created");
            }
        };
        
        editorWindow.Show();
    }

    [RelayCommand]
    private void EditJob()
    {
        if (SelectedJob == null)
        {
            StatusMessage = LanguageManager.Instance.GetString("Job_NoSelected");
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
            StatusMessage = LanguageManager.Instance.GetString("Job_NoSelected");
            return;
        }

        try
        {
            // Save job name before clearing selection
            string jobName = SelectedJob.Name;
            int index = Jobs.IndexOf(SelectedJob);
            
            JobManager.DeleteJob(index);
            Jobs.Remove(SelectedJob);
            
            StatusMessage = $"Job '{jobName}' deleted";
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
        ProgressPercentage = 0;
        ProgressText = string.Empty;
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
            List<string> businessSoftwareList = AppConfig.GetBusinessSoftwareNames();
            string? businessSoftware = businessSoftwareList.Count > 0 ? string.Join(";", businessSoftwareList) : null;
            
            foreach (var job in SelectedJobs.ToList())
            {
                try
                {
                    StatusMessage = $"Executing: {job.Name} ({successCount + failCount + 1}/{totalJobs})...";
                    
                    // Create progress callback to update UI
                    var progressReporter = new Progress<(int filesProcessed, int totalFiles)>(progressData =>
                    {
                        var (filesProcessed, totalFiles) = progressData;
                        // Calculate percentage (0-99, not 100 to show completion differently)
                        ProgressPercentage = totalFiles > 0 ? Math.Min((filesProcessed * 99.0) / totalFiles, 99) : 0;
                        ProgressText = $"Processing file {filesProcessed} of {totalFiles}";
                    });
                    
                    await Task.Run(() => backupService.ExecuteBackup(job, businessSoftware, progressReporter));
                    
                    // Set to 100% on completion
                    ProgressPercentage = 100;
                    ProgressText = "Completed!";
                    
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
            StatusMessage = LanguageManager.Instance.GetString("Job_NoJobsToExecute");
            return;
        }

        IsExecuting = true;
        StatusMessage = string.Format(LanguageManager.Instance.GetString("Job_ExecutingAll"), Jobs.Count);

        try
        {
            // Create backup service with encryption support
            string cryptoSoftPath = AppConfig.GetCryptoSoftPath();
            List<string> extensionsToEncrypt = AppConfig.GetExtensionsToEncrypt();
            var backupService = new BackupService(_logger, cryptoSoftPath, extensionsToEncrypt);
            List<string> businessSoftwareList = AppConfig.GetBusinessSoftwareNames();
            string? businessSoftware = businessSoftwareList.Count > 0 ? string.Join(";", businessSoftwareList) : null;
            
            int successCount = 0;
            int failCount = 0;
            int totalJobs = Jobs.Count;

            foreach (var job in Jobs.ToList())
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
                    await Task.Delay(1000);
                }
                catch (UnauthorizedAccessException ex)
                {
                    failCount++;
                    StatusMessage = $"Failed: {job.Name} - Access denied: {ex.Message}";
                    await Task.Delay(1000);
                }
                catch (Exception ex) when (ex is not OutOfMemoryException && ex is not StackOverflowException)
                {
                    failCount++;
                    StatusMessage = $"Failed: {job.Name} - {ex.Message}";
                    await Task.Delay(1000);
                }
            }

            StatusMessage = $"Completed: {successCount} succeeded, {failCount} failed";
        }
        catch (Exception ex) when (ex is not OutOfMemoryException && ex is not StackOverflowException)
        {
            // Unexpected error during service initialization
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
                StatusMessage = LanguageManager.Instance.GetString("Settings_Saved");
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
