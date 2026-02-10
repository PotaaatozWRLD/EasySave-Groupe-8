using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySave.GUI.Services;

/// <summary>
/// Language service implementation using JSON files for translations.
/// </summary>
public class LanguageService : ILanguageService
{
    private readonly string _languagesPath;
    private Dictionary<string, string> _currentTranslations = new();
    private string _currentLanguage = "en";

    public string CurrentLanguage => _currentLanguage;
    public event EventHandler? LanguageChanged;

    public LanguageService()
    {
        // Use existing language files from Console project
        _languagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");
        
        // Load default language
        LoadLanguage("en");
    }

    public string GetString(string key)
    {
        return _currentTranslations.TryGetValue(key, out var value) ? value : $"[{key}]";
    }

    public void ChangeLanguage(string languageCode)
    {
        if (_currentLanguage == languageCode)
            return;

        if (LoadLanguage(languageCode))
        {
            _currentLanguage = languageCode;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool LoadLanguage(string languageCode)
    {
        try
        {
            string filePath = Path.Combine(_languagesPath, $"lang.{languageCode}.json");
            
            if (!File.Exists(filePath))
                return false;

            string json = File.ReadAllText(filePath);
            var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            
            if (translations != null)
            {
                _currentTranslations = translations;
                return true;
            }
        }
        catch
        {
            // Fallback to empty translations
        }

        return false;
    }
}
