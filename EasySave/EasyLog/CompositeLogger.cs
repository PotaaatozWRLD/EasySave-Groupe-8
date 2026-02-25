using System.Collections.Generic;
using EasySave.Shared;

namespace EasyLog;

/// <summary>
/// Composite logger that forwards log calls to multiple logger implementations.
/// Allows logging to file and network simultaneously.
/// </summary>
public class CompositeLogger : ILogger, IDisposable
{
    private readonly List<ILogger> _loggers;

    public CompositeLogger(IEnumerable<ILogger> loggers)
    {
        _loggers = new List<ILogger>(loggers);
    }

    public void WriteLog(LogEntry logEntry)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.WriteLog(logEntry);
            }
            catch
            {
                // Prevent one failing logger from breaking the others
            }
        }
    }

    public void UpdateState(StateEntry stateEntry)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.UpdateState(stateEntry);
            }
            catch
            {
                // Prevent one failing logger from breaking the others
            }
        }
    }

    public void Dispose()
    {
        foreach (var logger in _loggers)
        {
            if (logger is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
