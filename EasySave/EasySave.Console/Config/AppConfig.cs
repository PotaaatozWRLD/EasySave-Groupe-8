using System.Text.Json;
using EasyLog;

namespace EasySave.ConsoleApp.Config;

public class AppConfig
{
    private const string ConfigFilePath = "config.json";
    private static AppConfig? _config;
    private static string _configPath = string.Empty;
    
    public string Language { get; set; } = "en";
    
    /// <summary>
    /// Log format preference (JSON or XML). Default: JSON (backward compatible with v1.0).
    /// </summary>
    public string LogFormatString { get; set; } = "JSON";
    
    /// <summary>
    /// Gets the log format as LogFormat enum.
    /// </summary>
    public LogFormat LogFormat => Enum.TryParse<LogFormat>(LogFormatString, true, out var format) ? format : LogFormat.JSON;

    private static void Load()
    {
        if (_config == null)
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProSoft",
                "EasySave"
            );
            _configPath = Path.Combine(appDataPath, ConfigFilePath);
            
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }
            else
            {
                _config = new AppConfig();
            }
        }
    }

    public static string GetLanguage()
    {
        Load();
        return _config!.Language;
    }
    
    /// <summary>
    /// Gets the current log format preference.
    /// </summary>
    public static LogFormat GetLogFormat()
    {
        Load();
        return _config!.LogFormat;
    }
    
    /// <summary>
    /// Sets the log format preference.
    /// </summary>
    /// <param name="format">The desired log format (JSON or XML).</param>
    public static void SetLogFormat(LogFormat format)
    {
        Load();
        _config!.LogFormatString = format.ToString();
        Save();
    }

    public static bool HasLanguageConfigured()
    {
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProSoft",
            "EasySave"
        );
        _configPath = Path.Combine(appDataPath, ConfigFilePath);
        return File.Exists(_configPath);
    }

    public static void SetLanguage(string language)
    {
        Load();
        _config!.Language = language;
        Save();
    }

    private static void Save()
    {
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProSoft",
            "EasySave"
        );
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }
        
        string fullPath = Path.Combine(appDataPath, ConfigFilePath);
        
        string json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fullPath, json);
    }
}
