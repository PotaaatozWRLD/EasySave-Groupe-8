using System.Text.Json;

namespace EasySave.ConsoleApp.Config;

public class AppConfig
{
    private const string ConfigFilePath = "config.json";
    private static AppConfig? _config;
    private static string _configPath = string.Empty;
    
    public string Language { get; set; } = "en";

    private static void Load()
    {
        if (_config == null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _configPath = Path.Combine(baseDirectory, ConfigFilePath);
            
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

    public static bool HasLanguageConfigured()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _configPath = Path.Combine(baseDirectory, ConfigFilePath);
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
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(baseDirectory, ConfigFilePath);
        
        string json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fullPath, json);
    }
}
