using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace EasySave.GUI.Helpers;

/// <summary>
/// Manages application localization using .NET resource files (.resx).
/// Implements INotifyPropertyChanged to enable reactive UI updates.
/// </summary>
public class LocalizationManager : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationManager> _instance = new(() => new LocalizationManager());
    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;
    private string _currentLanguageCode = "en";

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the singleton instance of LocalizationManager.
    /// </summary>
    public static LocalizationManager Instance => _instance.Value;

    private LocalizationManager()
    {
        _resourceManager = new ResourceManager("EasySave.GUI.Resources.Strings", typeof(LocalizationManager).Assembly);
        // Load language from config at startup
        string lang = GetLanguageFromConfig();
        _currentCulture = GetCultureInfo(lang);
        _currentLanguageCode = lang;
        CultureInfo.CurrentUICulture = _currentCulture;
    }

    /// <summary>
    /// Gets the current culture code (e.g., "en", "fr").
    /// </summary>
    public string CurrentCulture => _currentCulture.TwoLetterISOLanguageName;
    private static string GetLanguageFromConfig()
    {
        try
        {
            // Try to use AppConfig from Console project (shared config)
            var appConfigType = Type.GetType("EasySave.ConsoleApp.Config.AppConfig, EasySave.Console");
            if (appConfigType != null)
            {
                var getLangMethod = appConfigType.GetMethod("GetLanguage");
                if (getLangMethod != null)
                {
                    return getLangMethod.Invoke(null, null)?.ToString() ?? "en";
                }
            }
        }
        catch { }
        return "en";
    }

    private static CultureInfo GetCultureInfo(string lang)
    {
        return lang.ToLowerInvariant() switch
        {
            "fr" => new CultureInfo("fr-FR"),
            "en" => new CultureInfo("en-US"),
            _ => new CultureInfo("en-US")
        };
    }

    /// <summary>
    /// Gets a localized string by its resource key.
    /// </summary>
    /// <param name="key">Resource key (e.g., "Menu_File")</param>
    /// <returns>Localized string or key if not found</returns>
    public string this[string key]
    {
        get
        {
            try
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                if (!string.IsNullOrEmpty(value))
                    return value;
                // Fallback to English if missing
                if (_currentCulture.TwoLetterISOLanguageName != "en")
                {
                    var fallback = _resourceManager.GetString(key, new CultureInfo("en-US"));
                    if (!string.IsNullOrEmpty(fallback))
                        return fallback;
                }
                return $"[{key}]";
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                return $"[{key}]";
            }
            catch (InvalidOperationException)
            {
                return $"[{key}]";
            }
        }
    }

    /// <summary>
    /// Changes the application language and notifies all bound UI elements.
    /// </summary>
    /// <param name="culture">Culture code (e.g., "en", "fr")</param>
    public void ChangeLanguage(string culture)
    {
        var newCulture = GetCultureInfo(culture);
        if (_currentCulture.TwoLetterISOLanguageName == newCulture.TwoLetterISOLanguageName)
            return;
        _currentCulture = newCulture;
        _currentLanguageCode = culture;
        CultureInfo.CurrentUICulture = newCulture;
        // Save to config if possible
        try
        {
            var appConfigType = Type.GetType("EasySave.ConsoleApp.Config.AppConfig, EasySave.Console");
            if (appConfigType != null)
            {
                var setLangMethod = appConfigType.GetMethod("SetLanguage");
                if (setLangMethod != null)
                {
                    setLangMethod.Invoke(null, new object[] { culture });
                }
            }
        }
        catch { }
        // Notify all properties changed to refresh all UI bindings
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    /// <summary>
    /// Toggles between English and French.
    /// </summary>
    public void ToggleLanguage()
    {
        string newLang = CurrentCulture == "en" ? "fr" : "en";
        ChangeLanguage(newLang);
    }
}
