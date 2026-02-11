using System.Text.Json;
using EasySave.ConsoleApp.Config;

namespace EasySave.ConsoleApp.Languages;

public class LanguageManager
{
    private static readonly Lazy<LanguageManager> _instance = new Lazy<LanguageManager>(() => new LanguageManager());
    
    private Dictionary<string, string> _strings = new();
    private string _currentLanguage = "en";

    private LanguageManager()
    {
        // Load language from config
        string language = AppConfig.GetLanguage();
        LoadLanguage(language);
    }

    public static LanguageManager Instance => _instance.Value;

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
        // Always reload language from config to ensure up-to-date language (workaround for GUI bug)
        string language = AppConfig.GetLanguage();
        if (language != _currentLanguage)
        {
            LoadLanguage(language);
        }
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
