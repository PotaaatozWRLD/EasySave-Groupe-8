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
    /// v2.0: List of file extensions to encrypt (e.g., ".docx", ".xlsx"). Default: empty (no encryption).
    /// </summary>
    public List<string> ExtensionsToEncrypt { get; set; } = new();
    
    /// <summary>
    /// v2.0: List of business software process names to detect (e.g., "calc", "notepad"). Default: empty (no detection).
    /// </summary>
    public List<string> BusinessSoftwareNames { get; set; } = new();
    
    /// <summary>
    /// [DEPRECATED] v2.0: Name of business software process to detect. Use BusinessSoftwareNames instead.
    /// Kept for backward compatibility.
    /// </summary>
    public string BusinessSoftwareName { get; set; } = string.Empty;
    
    /// <summary>
    /// v3.0: List of priority file extensions (e.g., ".docx", ".xlsx"). These files are processed first.
    /// </summary>
    public List<string> PriorityExtensions { get; set; } = new();
    
    /// <summary>
    /// v3.0: Maximum file size in KB for parallel transfer throttling. Files larger than this transfer one at a time.
    /// </summary>
    public long MaxLargeFileSizeKB { get; set; } = 1024; // Default: 1 MB
    
    /// <summary>
    /// [DEPRECATED] v3.0: Use MaxLargeFileSizeKB instead (in KB, not bytes).
    /// </summary>
    public long MaxLargeFileSize { get; set; } = 0;
    
    /// <summary>
    /// v2.0: Path to CryptoSoft.exe executable. Default: relative path to CryptoSoft in project.
    /// </summary>
    public string CryptoSoftPath { get; set; } = "CryptoSoft\\bin\\Release\\net10.0\\CryptoSoft.exe";
    
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
    
    /// <summary>
    /// v2.0: Gets the list of file extensions to encrypt.
    /// </summary>
    public static List<string> GetExtensionsToEncrypt()
    {
        Load();
        return _config!.ExtensionsToEncrypt;
    }
    
    /// <summary>
    /// v2.0: Sets the list of file extensions to encrypt.
    /// </summary>
    /// <param name="extensions">List of extensions (must start with dot, e.g., ".docx").</param>
    public static void SetExtensionsToEncrypt(List<string> extensions)
    {
        Load();
        // Validate: all extensions must start with dot
        foreach (var ext in extensions)
        {
            if (!ext.StartsWith("."))
            {
                throw new ArgumentException($"Extension '{ext}' must start with a dot (e.g., '.docx')");
            }
        }
        _config!.ExtensionsToEncrypt = extensions;
        Save();
    }
    
    /// <summary>
    /// v2.0: Gets the list of business software process names to detect.
    /// </summary>
    public static List<string> GetBusinessSoftwareNames()
    {
        Load();
        // Migration: if old single value exists and list is empty, migrate it
        if (_config!.BusinessSoftwareNames.Count == 0 && !string.IsNullOrWhiteSpace(_config.BusinessSoftwareName))
        {
            _config.BusinessSoftwareNames.Add(_config.BusinessSoftwareName);
            Save();
        }
        return _config!.BusinessSoftwareNames;
    }
    
    /// <summary>
    /// v2.0: Sets the list of business software process names to detect.
    /// </summary>
    /// <param name="processNames">List of process names without extension (e.g., "calc", not "calc.exe").</param>
    public static void SetBusinessSoftwareNames(List<string> processNames)
    {
        Load();
        _config!.BusinessSoftwareNames = processNames;
        Save();
    }
    
    /// <summary>
    /// [DEPRECATED] v2.0: Gets the business software process name to detect. Use GetBusinessSoftwareNames instead.
    /// </summary>
    public static string GetBusinessSoftwareName()
    {
        Load();
        var names = GetBusinessSoftwareNames();
        return names.Count > 0 ? names[0] : string.Empty;
    }
    
    /// <summary>
    /// [DEPRECATED] v2.0: Sets the business software process name to detect. Use SetBusinessSoftwareNames instead.
    /// </summary>
    /// <param name="processName">Process name without extension (e.g., "calc", not "calc.exe").</param>
    public static void SetBusinessSoftwareName(string processName)
    {
        Load();
        _config!.BusinessSoftwareName = processName;
        _config.BusinessSoftwareNames = !string.IsNullOrWhiteSpace(processName)
            ? new List<string> { processName }
            : new List<string>();
        Save();
    }
    
    /// <summary>
    /// v3.0: Gets the maximum large file size threshold for parallel transfers.
    /// </summary>
    public static long GetMaxLargeFileSize()
    {
        Load();
        return _config!.MaxLargeFileSize;
    }
    
    /// <summary>
    /// v3.0: Sets the maximum large file size threshold for parallel transfers.
    /// </summary>
    /// <param name="sizeInBytes">File size in bytes (0 = no limit).</param>
    public static void SetMaxLargeFileSize(long sizeInBytes)
    {
        Load();
        _config!.MaxLargeFileSize = sizeInBytes;
        Save();
    }
    
    /// <summary>
    /// v2.0: Gets the path to CryptoSoft.exe executable.
    /// </summary>
    public static string GetCryptoSoftPath()
    {
        Load();
        return _config!.CryptoSoftPath;
    }
    
    /// <summary>
    /// v2.0: Sets the path to CryptoSoft.exe executable.
    /// </summary>
    /// <param name="path">Absolute or relative path to CryptoSoft.exe.</param>
    public static void SetCryptoSoftPath(string path)
    {
        Load();
        _config!.CryptoSoftPath = path;
        Save();
    }
    
    /// <summary>
    /// v3.0: Gets the list of priority file extensions.
    /// </summary>
    public static List<string> GetPriorityExtensions()
    {
        Load();
        return _config!.PriorityExtensions;
    }
    
    /// <summary>
    /// v3.0: Sets the list of priority file extensions.
    /// </summary>
    /// <param name="extensions">List of extensions (must start with dot, e.g., ".docx").</param>
    public static void SetPriorityExtensions(List<string> extensions)
    {
        Load();
        // Validate: all extensions must start with dot
        var invalidExt = extensions.FirstOrDefault(ext => !ext.StartsWith("."));
        if (invalidExt != null)
        {
            throw new ArgumentException($"Extension '{invalidExt}' must start with a dot (e.g., '.docx')");
        }
        _config!.PriorityExtensions = extensions;
        Save();
    }
    
    /// <summary>
    /// v3.0: Gets the maximum large file size threshold in KB for parallel transfers.
    /// </summary>
    public static long GetMaxLargeFileSizeKB()
    {
        Load();
        return _config!.MaxLargeFileSizeKB;
    }
    
    /// <summary>
    /// v3.0: Sets the maximum large file size threshold in KB for parallel transfers.
    /// </summary>
    /// <param name="sizeInKB">File size in kilobytes (0 = no limit).</param>
    public static void SetMaxLargeFileSizeKB(long sizeInKB)
    {
        Load();
        _config!.MaxLargeFileSizeKB = sizeInKB;
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
        
        // Retry write in case of concurrent test access
        for (int i = 0; i < 3; i++)
        {
            try
            {
                File.WriteAllText(fullPath, json);
                break;
            }
            catch (IOException) when (i < 2)
            {
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
