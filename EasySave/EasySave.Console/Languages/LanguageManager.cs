using System.Text.Json;
using EasySave.ConsoleApp.Config;

namespace EasySave.ConsoleApp.Languages;

public class LanguageManager
{
    private static LanguageManager? _instance;
    private static readonly object _lock = new object();
    
    private Dictionary<string, string> _strings = new();
    private string _currentLanguage = "en";

    private LanguageManager()
    {
        // Load language from config
        string language = AppConfig.GetLanguage();
        LoadLanguage(language);
    }

    public static LanguageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new LanguageManager();
                    }
                }
            }
            return _instance;
        }
    }

    public string CurrentLanguage => _currentLanguage;

    private void LoadLanguage(string culture)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string langFilePath = Path.Combine(baseDirectory, "Languages", $"lang.{culture}.json");
        
        if (File.Exists(langFilePath))
        {
            string json = File.ReadAllText(langFilePath);
            _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            _currentLanguage = culture;
        }
        else
        {
            // Fallback to English if language file not found
            langFilePath = Path.Combine(baseDirectory, "Languages", "lang.en.json");
            if (File.Exists(langFilePath))
            {
                string json = File.ReadAllText(langFilePath);
                _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                _currentLanguage = "en";
            }
        }
    }

    public string GetString(string key)
    {
        return _strings.TryGetValue(key, out string? value) ? value : $"[{key}]";
    }

    public void ChangeLanguage(string culture)
    {
        LoadLanguage(culture);
        
        // Save to config
        AppConfig.SetLanguage(culture);
    }

    public void ToggleLanguage()
    {
        string newLang = _currentLanguage == "en" ? "fr" : "en";
        ChangeLanguage(newLang);
    }
}
