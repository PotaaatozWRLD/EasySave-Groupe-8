using EasySave.Shared;

namespace EasySave.Core.Models;

/// <summary>
/// Context for managing the execution of a backup job in V3.0.
/// Handles Play/Pause/Stop controls and parallel execution.
/// </summary>
public class JobExecutionContext
{
    /// <summary>
    /// The backup job being executed
    /// </summary>
    public BackupJob Job { get; set; }
    
    /// <summary>
    /// Current execution state (Running, Paused, Stopped, etc.)
    /// </summary>
    public JobExecutionState State { get; set; }
    
    /// <summary>
    /// Cancellation token for Stop command (immediate termination)
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; }
    
    /// <summary>
    /// Manual reset event for Pause/Resume functionality
    /// When reset (false), job is paused. When set (true), job can continue.
    /// </summary>
    public ManualResetEventSlim PauseEvent { get; set; }
    
    /// <summary>
    /// Current progress percentage (0-100)
    /// </summary>
    public double ProgressPercentage { get; set; }
    
    /// <summary>
    /// Number of files processed so far
    /// </summary>
    public int FilesProcessed { get; set; }
    
    /// <summary>
    /// Total number of files to process
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// Initializes a new job execution context
    /// </summary>
    public JobExecutionContext(BackupJob job)
    {
        Job = job;
        State = JobExecutionState.Running;
        CancellationTokenSource = new CancellationTokenSource();
        PauseEvent = new ManualResetEventSlim(true); // Initially running (not paused)
        ProgressPercentage = 0;
        FilesProcessed = 0;
        TotalFiles = 0;
    }

    /// <summary>
    /// Pauses the job execution (will pause after current file)
    /// </summary>
    public void Pause()
    {
        State = JobExecutionState.Paused;
        Job.State = "Paused";
        PauseEvent.Reset(); // Blocks execution
    }

    /// <summary>
    /// Resumes a paused job
    /// </summary>
    public void Resume()
    {
        State = JobExecutionState.Running;
        Job.State = "Active";
        PauseEvent.Set(); // Allows execution to continue
    }

    /// <summary>
    /// Stops the job immediately (cancels execution)
    /// </summary>
    public void Stop()
    {
        State = JobExecutionState.Stopped;
        Job.State = "Stopped";
        CancellationTokenSource.Cancel();
        PauseEvent.Set(); // Unblock if paused so it can check cancellation
    }

    /// <summary>
    /// Checks if job should pause or has been cancelled.
    /// Call this between file operations.
    /// </summary>
    public void CheckPauseAndCancellation()
    {
        // Wait if paused
        PauseEvent.Wait();
        
        // Throw if cancelled
        CancellationTokenSource.Token.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Updates progress information
    /// </summary>
    public void UpdateProgress(int filesProcessed, int totalFiles)
    {
        FilesProcessed = filesProcessed;
        TotalFiles = totalFiles;
        ProgressPercentage = totalFiles > 0 ? (filesProcessed * 100.0) / totalFiles : 0;
    }

    /// <summary>
    /// Disposes resources
    /// </summary>
    public void Dispose()
    {
        CancellationTokenSource?.Dispose();
        PauseEvent?.Dispose();
    }
}
