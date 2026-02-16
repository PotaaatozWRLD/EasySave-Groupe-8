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

    public ParallelBackupCoordinator(ILogger logger, string cryptoSoftPath, List<string> extensionsToEncrypt)
    {
        _logger = logger;
        _cryptoSoftPath = cryptoSoftPath;
        _extensionsToEncrypt = extensionsToEncrypt;
        _activeJobs = new ConcurrentDictionary<string, JobExecutionContext>();
    }

    /// <summary>
    /// Executes multiple backup jobs in parallel.
    /// V3.0: Jobs run concurrently instead of sequentially.
    /// </summary>
    public async Task ExecuteJobsInParallelAsync(
        IEnumerable<BackupJob> jobs,
        string? businessSoftware = null,
        IProgress<(string jobName, int filesProcessed, int totalFiles)>? progress = null)
    {
        var jobTasks = new List<Task>();

        foreach (var job in jobs)
        {
            // Create execution context for this job
            var context = new JobExecutionContext(job);
            _activeJobs.TryAdd(job.Name, context);

            // Start job execution in parallel
            var jobTask = Task.Run(async () =>
            {
                try
                {
                    var backupService = new BackupService(_logger, _cryptoSoftPath, _extensionsToEncrypt);
                    
                    // Create progress reporter for this specific job
                    var jobProgress = new Progress<(int filesProcessed, int totalFiles)>(p =>
                    {
                        context.UpdateProgress(p.filesProcessed, p.totalFiles);
                        progress?.Report((job.Name, p.filesProcessed, p.totalFiles));
                    });

                    // Execute backup with context for pause/stop support
                    await ExecuteSingleJobAsync(backupService, context, businessSoftware, jobProgress);
                    
                    context.State = JobExecutionState.Completed;
                }
                catch (OperationCanceledException)
                {
                    // Job was stopped
                    context.State = JobExecutionState.Stopped;
                }
                catch (Exception)
                {
                    // Job failed
                    context.State = JobExecutionState.Failed;
                    throw;
                }
                finally
                {
                    // Cleanup
                    _activeJobs.TryRemove(job.Name, out _);
                }
            });

            jobTasks.Add(jobTask);
        }

        // Wait for all jobs to complete
        await Task.WhenAll(jobTasks);
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
    /// Resumes a paused job by name
    /// </summary>
    public void ResumeJob(string jobName)
    {
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
    /// Resumes all paused jobs
    /// </summary>
    public void ResumeAllJobs()
    {
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
