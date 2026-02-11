namespace EasyLog;

/// <summary>
/// Log format enumeration.
/// </summary>
public enum LogFormat
{
    /// <summary>
    /// JSON format (default, backward compatible with v1.0).
    /// </summary>
    JSON,

    /// <summary>
    /// XML format (added in v1.1 for client requirements).
    /// </summary>
    XML
}

/// <summary>
/// Factory class for creating logger instances based on desired format.
/// Implements Factory Pattern for extensibility.
/// </summary>
public static class LoggerFactory
{
    /// <summary>
    /// Creates a logger instance based on the specified format.
    /// </summary>
    /// <param name="format">The desired log format (JSON or XML).</param>
    /// <param name="logDirectory">Directory where daily log files will be stored.</param>
    /// <param name="stateFilePath">Path to the real-time state file.</param>
    /// <returns>An ILogger implementation (JsonLogger or XmlLogger).</returns>
    /// <exception cref="ArgumentException">Thrown when an unsupported format is specified.</exception>
    public static ILogger CreateLogger(LogFormat format, string logDirectory, string stateFilePath)
    {
        return format switch
        {
            LogFormat.JSON => new JsonLogger(logDirectory, stateFilePath),
            LogFormat.XML => new XmlLogger(logDirectory, stateFilePath),
            _ => throw new ArgumentException($"Unsupported log format: {format}", nameof(format))
        };
    }
}
