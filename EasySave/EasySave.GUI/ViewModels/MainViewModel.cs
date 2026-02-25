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
        var fileLogger = LoggerFactory.CreateLogger(logFormat, _logPath, _statePath);

        // V3.0: Centralized logging — 3 modes defined by spec:
        //   "Local"  → logs only on this PC (default)
        //   "Docker" → logs only on Docker server
        //   "Both"   → logs on this PC AND Docker server
        string mode = AppConfig.GetLoggingMode();

        if (mode == "Docker" || mode == "Both")
        {
            try
            {
                string ip = AppConfig.GetLogServerIp();
                int port = AppConfig.GetLogServerPort();
                var networkLogger = new NetworkLogger(ip, port);

                _logger = mode == "Both"
                    ? new CompositeLogger(new List<ILogger> { fileLogger, networkLogger })
                    : networkLogger; // Docker-only: no local file
            }
            catch (Exception ex)
            {
                // Fallback to local if Docker init fails
                _logger = fileLogger;
                _logger.WriteLog(new LogEntry
                {
                    JobName = "System",
                    ErrorMessage = $"Failed to initialize network logger: {ex.Message}",
                    Timestamp = DateTime.Now
                });
            }
        }
        else
        {
            _logger = fileLogger; // Local only
        }
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

    private ParallelBackupCoordinator? _coordinator;
    private readonly object _lock = new();

    [RelayCommand]
    private async Task ExecuteSelectedJobAsync()
    {
        if (SelectedJobs.Count == 0)
        {
            StatusMessage = LanguageManager.Instance.GetString("Job_NoSelected");
            return;
        }

        await ExecuteJobsInternalAsync(SelectedJobs.ToList());
    }

    [RelayCommand]
    private async Task ExecuteAllJobsAsync()
    {
        if (Jobs.Count == 0)
        {
            StatusMessage = LanguageManager.Instance.GetString("Job_NoJobsToExecute");
            return;
        }

        await ExecuteJobsInternalAsync(Jobs.ToList());
    }

    private async Task ExecuteJobsInternalAsync(List<BackupJob> jobsToExecute)
    {
        if (_logger == null) return;
        
        // Ensure coordinator is initialized
        if (_coordinator == null)
        {
            string cryptoSoftPath = AppConfig.GetCryptoSoftPath();
            List<string> extensionsToEncrypt = AppConfig.GetExtensionsToEncrypt();
            List<string> businessSoftwareList = AppConfig.GetBusinessSoftwareNames();
            _coordinator = new ParallelBackupCoordinator(_logger, cryptoSoftPath, extensionsToEncrypt, businessSoftwareList);
            
            // Subscribe to busy state changes to toggle global control buttons
            _coordinator.IsBusyChanged += (isBusy) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IsExecuting = isBusy;
                    // Also clear global progress when done
                    if (!isBusy)
                    {
                        ProgressPercentage = 0;
                        ProgressText = "All jobs completed";
                        StatusMessage = LanguageManager.Instance.GetString("Status_Completed");
                    }
                });
            };

            // Subscribe to Business Software detection
            _coordinator.BusinessSoftwareDetected += (processName) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    StatusMessage = $"⚠️ PAUSED: Business Software Detected ({processName})";
                });
            };
        }

        // Filter out jobs that are already running
        var jobsToStart = jobsToExecute.Where(j => !j.IsRunning).ToList();
        
        if (jobsToStart.Count == 0) return;

        StatusMessage = string.Format(LanguageManager.Instance.GetString("Job_ExecutingAll"), jobsToStart.Count);

        try
        {
            // Setup progress reporting (legacy, mostly unused now as jobs update themselves)
            var progress = new Progress<(string jobName, int filesProcessed, int totalFiles)>(data =>
            {
               // Global progress logic if needed
            });

            // Start jobs without blocking the UI thread
            await _coordinator.StartJobsAsync(jobsToStart, progress);
            
            // Note: We don't set IsExecuting = true globally anymore, 
            // nor do we wait for completion here.
            // But we might want to flag that *something* is running?
            // For now, IsExecuting is removed from blocking logic so we can leave it false or unused.
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error starting execution: {ex.Message}";
        }
    }

    [RelayCommand]
    private void PauseJob(BackupJob job)
    {
        _coordinator?.PauseJob(job.Name);
        StatusMessage = string.Format(LocalizationManager.Instance["Status_Paused"], job.Name);
    }

    [RelayCommand]
    private void ResumeJob(BackupJob job)
    {
        _coordinator?.ResumeJob(job.Name);
        StatusMessage = string.Format(LocalizationManager.Instance["Status_Resumed"], job.Name);
    }

    [RelayCommand]
    private void StopJob(BackupJob job)
    {
        _coordinator?.StopJob(job.Name);
        StatusMessage = string.Format(LocalizationManager.Instance["Status_Stopped"], job.Name);
    }

    [RelayCommand]
    private void PauseAll()
    {
        _coordinator?.PauseAllJobs();
        StatusMessage = LocalizationManager.Instance["Status_AllPaused"];
    }

    [RelayCommand]
    private void ResumeAll()
    {
        _coordinator?.ResumeAllJobs();
        StatusMessage = LocalizationManager.Instance["Status_AllResumed"];
    }

    [RelayCommand]
    private void StopAll()
    {
        _coordinator?.StopAllJobs();
        StatusMessage = LocalizationManager.Instance["Status_StoppingAll"];
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
                
                // V3.0: Update running coordinator with new configuration
                if (_coordinator != null)
                {
                    List<string> extensionsToEncrypt = AppConfig.GetExtensionsToEncrypt();
                    List<string> businessSoftwareList = AppConfig.GetBusinessSoftwareNames();
                    _coordinator.UpdateConfiguration(extensionsToEncrypt, businessSoftwareList);
                }
                
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
