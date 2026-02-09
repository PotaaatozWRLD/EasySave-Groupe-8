using System.Xml;
using System.Xml.Serialization;

namespace EasyLog;

/// <summary>
/// XML logger implementation for EasyLog.
/// Writes log entries and state entries to XML files with indentation.
/// </summary>
public class XmlLogger : ILogger
{
    private readonly string _logDirectory;
    private readonly string _stateFilePath;

    /// <summary>
    /// Initializes a new instance of the XmlLogger class.
    /// </summary>
    /// <param name="logDirectory">Directory where daily log files will be stored.</param>
    /// <param name="stateFilePath">Path to the real-time state file.</param>
    public XmlLogger(string logDirectory, string stateFilePath)
    {
        _logDirectory = logDirectory ?? throw new ArgumentNullException(nameof(logDirectory));
        
        // Use .xml extension for XML logger state file
        _stateFilePath = Path.ChangeExtension(stateFilePath, ".xml") ?? throw new ArgumentNullException(nameof(stateFilePath));

        // Create log directory if it doesn't exist
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    /// <summary>
    /// Writes a log entry for a file operation to daily XML log file.
    /// Implements ILogger.WriteLog() for XML format support.
    /// </summary>
    /// <param name="entry">The log entry to write.</param>
    public void WriteLog(LogEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        string logFileName = $"{entry.Timestamp:yyyy-MM-dd}.xml";
        string logFilePath = Path.Combine(_logDirectory, logFileName);

        try
        {
            // Read existing entries if file exists
            List<LogEntry> entries;
            if (File.Exists(logFilePath))
            {
                entries = DeserializeLogEntries(logFilePath);
            }
            else
            {
                entries = new List<LogEntry>();
            }

            // Add new entry
            entries.Add(entry);

            // Write all entries back to file
            SerializeLogEntries(logFilePath, entries);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Failed to write log entry to {logFilePath}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new InvalidOperationException($"Access denied to log file {logFilePath}", ex);
        }
    }

    /// <summary>
    /// Updates the real-time state file with current backup progress.
    /// Implements ILogger.UpdateState() for XML format support.
    /// </summary>
    /// <param name="state">The state entry to write.</param>
    public void UpdateState(StateEntry state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        try
        {
            // For state file, we maintain a list of all backup job states
            List<StateEntry> states;
            if (File.Exists(_stateFilePath))
            {
                states = DeserializeStateEntries(_stateFilePath);
            }
            else
            {
                states = new List<StateEntry>();
            }

            // Update existing state or add new one
            int existingIndex = states.FindIndex(s => s.Name == state.Name);
            if (existingIndex >= 0)
            {
                states[existingIndex] = state;
            }
            else
            {
                states.Add(state);
            }

            // Write all states back to file
            SerializeStateEntries(_stateFilePath, states);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Failed to write state to {_stateFilePath}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new InvalidOperationException($"Access denied to state file {_stateFilePath}", ex);
        }
    }

    /// <summary>
    /// Serializes log entries to XML file with indentation.
    /// </summary>
    private void SerializeLogEntries(string filePath, List<LogEntry> entries)
    {
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineOnAttributes = false
        };

        using (XmlWriter writer = XmlWriter.Create(filePath, settings))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>), new XmlRootAttribute("LogEntries"));
            serializer.Serialize(writer, entries);
        }
    }

    /// <summary>
    /// Deserializes log entries from XML file.
    /// </summary>
    private List<LogEntry> DeserializeLogEntries(string filePath)
    {
        try
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogEntry>), new XmlRootAttribute("LogEntries"));
                return (List<LogEntry>)serializer.Deserialize(reader)!;
            }
        }
        catch (XmlException)
        {
            // File is empty or corrupted, return empty list
            return new List<LogEntry>();
        }
        catch (InvalidOperationException)
        {
            // Deserialization failed, return empty list
            return new List<LogEntry>();
        }
    }

    /// <summary>
    /// Serializes state entries to XML file with indentation.
    /// </summary>
    private void SerializeStateEntries(string filePath, List<StateEntry> states)
    {
        // Ensure directory exists
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineOnAttributes = false
        };

        using (XmlWriter writer = XmlWriter.Create(filePath, settings))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<StateEntry>), new XmlRootAttribute("StateEntries"));
            serializer.Serialize(writer, states);
        }
    }

    /// <summary>
    /// Deserializes state entries from XML file.
    /// </summary>
    private List<StateEntry> DeserializeStateEntries(string filePath)
    {
        try
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<StateEntry>), new XmlRootAttribute("StateEntries"));
                return (List<StateEntry>)serializer.Deserialize(reader)!;
            }
        }
        catch (XmlException)
        {
            // File is empty or corrupted, return empty list
            return new List<StateEntry>();
        }
        catch (InvalidOperationException)
        {
            // Deserialization failed, return empty list
            return new List<StateEntry>();
        }
    }
}
