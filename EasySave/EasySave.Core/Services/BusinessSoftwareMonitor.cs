using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EasySave.Core.Services;

/// <summary>
/// Monitors business software processes in the background.
/// Raises events when software starts or stops to trigger auto-pause/resume.
/// </summary>
public class BusinessSoftwareMonitor : IDisposable
{
    private List<string> _processNames;
    private readonly Timer? _timer;
    private bool _isBusinessSoftwareRunning;
    private readonly object _lock = new();
    
    public event EventHandler? SoftwareStarted;
    public event EventHandler? SoftwareStopped;

    public bool IsBusinessSoftwareRunning
    {
        get { lock(_lock) return _isBusinessSoftwareRunning; }
    }

    public string? DetectedProcessName { get; private set; }

    public BusinessSoftwareMonitor(List<string> processNames, int checkIntervalMs = 2000)
    {
        _processNames = processNames ?? new List<string>();
        
        // Only start timer if there are processes to monitor
        if (_processNames.Count > 0)
        {
            _timer = new Timer(CheckProcesses, null, 0, checkIntervalMs);
        }
    }

    private void CheckProcesses(object? state)
    {
        try
        {
            string? runningProcess = BusinessSoftwareDetector.GetFirstRunningProcess(_processNames);
            bool isRunning = !string.IsNullOrEmpty(runningProcess);
            
            lock (_lock)
            {
                if (isRunning && !_isBusinessSoftwareRunning)
                {
                    _isBusinessSoftwareRunning = true;
                    DetectedProcessName = runningProcess;
                    SoftwareStarted?.Invoke(this, EventArgs.Empty);
                }
                else if (!isRunning && _isBusinessSoftwareRunning)
                {
                    _isBusinessSoftwareRunning = false;
                    DetectedProcessName = null;
                    SoftwareStopped?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        catch (Exception)
        {
            // Ignore errors during background check to prevent crash
        }
    }

    public void UpdateProcessNames(List<string> newNames)
    {
        lock (_lock)
        {
            _processNames = newNames ?? new List<string>();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
