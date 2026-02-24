using System.Collections.Concurrent;
using EasySave.Shared;
using EasySave.Core.Models;
using EasyLog;

namespace EasySave.Core.Services;

/// <summary>
/// Coordinates parallel execution of multiple backup jobs in V3.0.
/// Manages job states, provides Play/Pause/Stop controls, and reports progress.
/// </summary>
public class ParallelBackupCoordinator
{
    private readonly ILogger _logger;
    private readonly string _cryptoSoftPath;
    private readonly List<string> _extensionsToEncrypt;
    private readonly ConcurrentDictionary<string, JobExecutionContext> _activeJobs;

    private readonly BusinessSoftwareMonitor? _monitor;

    public ParallelBackupCoordinator(ILogger logger, string cryptoSoftPath, List<string> extensionsToEncrypt, List<string>? businessSoftware = null)
    {
        _logger = logger;
        _cryptoSoftPath = cryptoSoftPath;
        _extensionsToEncrypt = extensionsToEncrypt;
        _activeJobs = new ConcurrentDictionary<string, JobExecutionContext>();

        // V3.0: Initialize business software monitor if configured
        if (businessSoftware != null && businessSoftware.Count > 0)
        {
            _monitor = new BusinessSoftwareMonitor(businessSoftware);
            _monitor.SoftwareStarted += OnBusinessSoftwareStarted;
            _monitor.SoftwareStopped += OnBusinessSoftwareStopped;
        }
    }

    private void OnBusinessSoftwareStarted(object? sender, EventArgs e)
    {
        string processName = _monitor?.DetectedProcessName ?? "Unknown";
        _logger.WriteLog(new LogEntry 
        { 
            JobName = "SYSTEM", 
            ErrorMessage = $"Business software detected ({processName}). Pausing all jobs." 
        });
        
        BusinessSoftwareDetected?.Invoke(processName);
        PauseAllJobs();
    }

    private void OnBusinessSoftwareStopped(object? sender, EventArgs e)
    {
        _logger.WriteLog(new LogEntry 
        { 
            JobName = "SYSTEM", 
            ErrorMessage = "Business software closed. Resuming all jobs." 
        });
        ResumeAllJobs();
    }

    public event Action<bool>? IsBusyChanged;
    public event Action<string>? BusinessSoftwareDetected;

    /// <summary>
    /// V3.0: Updates the coordinator's configuration at runtime.
    /// This allows live updates for encryption extensions and business software without restart.
    /// </summary>
    public void UpdateConfiguration(List<string> extensionsToEncrypt, List<string>? businessSoftware)
    {
        _extensionsToEncrypt.Clear();
        if (extensionsToEncrypt != null)
        {
            _extensionsToEncrypt.AddRange(extensionsToEncrypt);
        }

        if (businessSoftware != null && businessSoftware.Count > 0)
        {
            if (_monitor == null)
            {
                // Initialize monitor if it didn't exist
                var monitor = new BusinessSoftwareMonitor(businessSoftware);
                monitor.SoftwareStarted += OnBusinessSoftwareStarted;
                monitor.SoftwareStopped += OnBusinessSoftwareStopped;
                
                // Use reflection or a private field if needed, but here we can just assign
                // Since _monitor is private, we can't easily swap it if it's readonly, 
                // let's check the declaration. It's not readonly.
                typeof(ParallelBackupCoordinator).GetField("_monitor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(this, monitor);
            }
            else
            {
                _monitor.UpdateProcessNames(businessSoftware);
            }
        }
        else
        {
            // If list is now empty, we might want to "stop" the monitor
            _monitor?.UpdateProcessNames(new List<string>());
        }
    }

    /// <summary>
    /// Starts execution of the provided jobs. 
    /// Can be called multiple times to add jobs to the running pool.
    /// </summary>
    public async Task StartJobsAsync(
        IEnumerable<BackupJob> jobs,
        IProgress<(string jobName, int filesProcessed, int totalFiles)>? progress = null)
    {
        var newJobs = jobs.Where(j => !_activeJobs.ContainsKey(j.Name)).ToList();

        if (newJobs.Count == 0) return;

        bool wasEmpty = _activeJobs.IsEmpty;

        // Ensure monitor is running if we have active jobs
        StartMonitorIfNeeded();

        foreach (var job in newJobs)
        {
            // Create execution context for this job
            var context = new JobExecutionContext(job);
            if (_activeJobs.TryAdd(job.Name, context))
            {
                job.State = "Active";

                // Checks if business software is already running at start
                if (_monitor != null && _monitor.IsBusinessSoftwareRunning)
                {
                    string processName = _monitor.DetectedProcessName ?? "Unknown";
                    _logger.WriteLog(new LogEntry
                    {
                        JobName = job.Name,
                        ErrorMessage = $"Paused on start: Business software running ({processName})"
                    });
                    
                    // Notify UI only once per batch or just fire it
                    BusinessSoftwareDetected?.Invoke(processName);
                    
                    context.Pause();
                } 
                job.Progress = 0;

                // Fire and forget (task tracked internally)
                _ = Task.Run(async () => await RunJobAsync(job, context, progress));
            }
        }
        
        if (wasEmpty && !_activeJobs.IsEmpty)
        {
            IsBusyChanged?.Invoke(true);
        }

        // We don't await completion here, we return so UI is free
        await Task.CompletedTask;
    }

    private async Task RunJobAsync(BackupJob job, JobExecutionContext context, IProgress<(string jobName, int filesProcessed, int totalFiles)>? progress)
    {
        try
        {
            var backupService = new BackupService(_logger, _cryptoSoftPath, _extensionsToEncrypt);
            
            var jobProgress = new Progress<(int filesProcessed, int totalFiles)>(p =>
            {
                context.UpdateProgress(p.filesProcessed, p.totalFiles);
                int percent = p.totalFiles > 0 ? (int)((p.filesProcessed * 100.0) / p.totalFiles) : 0;
                job.Progress = percent;
                progress?.Report((job.Name, p.filesProcessed, p.totalFiles));
            });

            await ExecuteSingleJobAsync(backupService, context, null, jobProgress);
            
            context.State = JobExecutionState.Completed;
            job.State = "Completed";
            job.Progress = 100;
        }
        catch (OperationCanceledException)
        {
            context.State = JobExecutionState.Stopped;
            job.State = "Stopped";
        }
        catch (IOException ex)
        {
            context.State = JobExecutionState.Failed;
            job.State = "Error";
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                ErrorMessage = $"I/O error during backup: {ex.Message}"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            context.State = JobExecutionState.Failed;
            job.State = "Error";
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                ErrorMessage = $"Access denied during backup: {ex.Message}"
            });
        }
        catch (InvalidOperationException ex)
        {
            context.State = JobExecutionState.Failed;
            job.State = "Error";
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                ErrorMessage = $"Invalid operation during backup: {ex.Message}"
            });
        }
        catch (Exception ex)
        {
            context.State = JobExecutionState.Failed;
            job.State = "Error";
            _logger.WriteLog(new LogEntry { JobName = job.Name, ErrorMessage = ex.Message });
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                ErrorMessage = $"Unexpected error during backup: {ex}"
            });
        }
        finally
        {
            _activeJobs.TryRemove(job.Name, out _);
            if (_activeJobs.IsEmpty)
            {
                IsBusyChanged?.Invoke(false);
            }
            StopMonitorIfNoJobs();
        }
    }

    private readonly object _monitorLock = new();
    private void StartMonitorIfNeeded()
    {
        lock (_monitorLock)
        {
            if (_monitor != null && !_monitor.IsBusinessSoftwareRunning) // Assuming simple check, acts as start trigger check
            {
                // Monitor is always instantiated in ctor if config present
                // We just need to ensure it's "active". 
                // In this design, Monitor runs its timer always if instantiated.
                // Or we can Start/Stop the monitor's internal timer if we exposed that.
                // For now, BusinessSoftwareMonitor runs consistently if created.
                // If we want to optimize, we would add Start()/Stop() to BusinessSoftwareMonitor.
                // But simplified: Monitor events are always subscribed.
            }
            
            // Re-subscribe if we unsubscribed? 
            // Better: Just keep it alive. The overhead is minimal (one timer).
        }
    }

    private void StopMonitorIfNoJobs()
    {
        // If we wanted to stop the monitor when no jobs are running.
    }

    /// <summary>
    /// Executes a single backup job with pause/stop support
    /// </summary>
    private async Task ExecuteSingleJobAsync(
        BackupService backupService,
        JobExecutionContext context,
        string? businessSoftware,
        IProgress<(int filesProcessed, int totalFiles)> progress)
    {
        await Task.Run(() =>
        {
            backupService.ExecuteBackupWithContext(
                context.Job,
                businessSoftware,
                progress,
                context);
        });
    }

    /// <summary>
    /// Pauses a specific job by name
    /// </summary>
    public void PauseJob(string jobName)
    {
        if (_activeJobs.TryGetValue(jobName, out var context))
        {
            context.Pause();
        }
    }

    /// <summary>
    /// Resumes a paused job by name, unless blocked by business software
    /// </summary>
    public void ResumeJob(string jobName)
    {
        if (_monitor != null && _monitor.IsBusinessSoftwareRunning)
        {
            string processName = _monitor.DetectedProcessName ?? "Unknown";
            _logger.WriteLog(new LogEntry
            {
                JobName = jobName,
                ErrorMessage = $"Resume blocked: Business software running ({processName})"
            });
            BusinessSoftwareDetected?.Invoke(processName);
            return;
        }

        if (_activeJobs.TryGetValue(jobName, out var context))
        {
            context.Resume();
        }
    }

    /// <summary>
    /// Stops a job immediately by name
    /// </summary>
    public void StopJob(string jobName)
    {
        if (_activeJobs.TryGetValue(jobName, out var context))
        {
            context.Stop();
        }
    }

    /// <summary>
    /// Pauses all currently running jobs
    /// </summary>
    public void PauseAllJobs()
    {
        foreach (var context in _activeJobs.Values)
        {
            context.Pause();
        }
    }

    /// <summary>
    /// Resumes all paused jobs, unless blocked by business software
    /// </summary>
    public void ResumeAllJobs()
    {
        if (_monitor != null && _monitor.IsBusinessSoftwareRunning)
        {
            string processName = _monitor.DetectedProcessName ?? "Unknown";
            _logger.WriteLog(new LogEntry
            {
                JobName = "SYSTEM",
                ErrorMessage = $"Resume All blocked: Business software running ({processName})"
            });
            BusinessSoftwareDetected?.Invoke(processName);
            return;
        }

        foreach (var context in _activeJobs.Values)
        {
            context.Resume();
        }
    }

    /// <summary>
    /// Stops all running jobs immediately
    /// </summary>
    public void StopAllJobs()
    {
        foreach (var context in _activeJobs.Values)
        {
            context.Stop();
        }
    }

    /// <summary>
    /// Gets the current state of a job
    /// </summary>
    public JobExecutionState? GetJobState(string jobName)
    {
        return _activeJobs.TryGetValue(jobName, out var context) ? context.State : null;
    }

    /// <summary>
    /// Gets all active jobs and their contexts
    /// </summary>
    public IReadOnlyDictionary<string, JobExecutionContext> GetActiveJobs()
    {
        return _activeJobs;
    }
}
