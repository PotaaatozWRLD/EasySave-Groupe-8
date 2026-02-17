using System.Collections.Concurrent;

namespace EasySave.Core.Services;

/// <summary>
/// V3.0: Manages throttling of large file transfers to prevent bandwidth saturation.
/// Only one large file can be transferred at a time across all backup jobs.
/// </summary>
public class LargeFileThrottle
{
    private static readonly SemaphoreSlim _largeFileSemaphore = new(1, 1);
    private readonly long _maxSizeKB;

    public LargeFileThrottle(long maxSizeKB)
    {
        _maxSizeKB = maxSizeKB;
    }

    /// <summary>
    /// Checks if a file is considered "large" based on configured threshold.
    /// </summary>
    public bool IsLargeFile(long fileSizeBytes)
    {
        if (_maxSizeKB == 0) return false; // No limit configured
        return fileSizeBytes > (_maxSizeKB * 1024);
    }

    /// <summary>
    /// Executes an action with large file throttling if applicable.
    /// If file is large, acquires semaphore (blocks if another large file is transferring).
    /// If file is small, executes immediately without blocking.
    /// </summary>
    public async Task ExecuteWithThrottlingAsync(long fileSizeBytes, Func<Task> action)
    {
        bool isLarge = IsLargeFile(fileSizeBytes);

        if (isLarge)
        {
            // Large file - acquire semaphore (only one large file at a time)
            await _largeFileSemaphore.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                _largeFileSemaphore.Release();
            }
        }
        else
        {
            // Small file - no throttling, execute immediately
            await action();
        }
    }

    /// <summary>
    /// Synchronous version for non-async operations.
    /// </summary>
    public void ExecuteWithThrottling(long fileSizeBytes, Action action)
    {
        bool isLarge = IsLargeFile(fileSizeBytes);

        if (isLarge)
        {
            _largeFileSemaphore.Wait();
            try
            {
                action();
            }
            finally
            {
                _largeFileSemaphore.Release();
            }
        }
        else
        {
            action();
        }
    }
}
