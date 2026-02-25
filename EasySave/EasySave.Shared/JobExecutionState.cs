namespace EasySave.Shared;

/// <summary>
/// Represents the execution state of a backup job in V3.0.
/// Used for Play/Pause/Stop controls.
/// </summary>
public enum JobExecutionState
{
    /// <summary>
    /// Job is currently running
    /// </summary>
    Running,
    
    /// <summary>
    /// Job is paused (waiting to resume)
    /// </summary>
    Paused,
    
    /// <summary>
    /// Job has been stopped (cancelled)
    /// </summary>
    Stopped,
    
    /// <summary>
    /// Job completed successfully
    /// </summary>
    Completed,
    
    /// <summary>
    /// Job failed with error
    /// </summary>
    Failed
}
