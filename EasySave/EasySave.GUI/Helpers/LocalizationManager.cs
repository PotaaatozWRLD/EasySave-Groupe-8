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

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the singleton instance of LocalizationManager.
    /// </summary>
    public static LocalizationManager Instance => _instance.Value;

    private LocalizationManager()
    {
        _resourceManager = new ResourceManager("EasySave.GUI.Resources.Strings", typeof(LocalizationManager).Assembly);
        _currentCulture = new CultureInfo("en-US");
        CultureInfo.CurrentUICulture = _currentCulture;
    }

    /// <summary>
    /// Gets the current culture code (e.g., "en", "fr").
    /// </summary>
    public string CurrentCulture => _currentCulture.TwoLetterISOLanguageName;

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
                return _resourceManager.GetString(key, _currentCulture) ?? $"[{key}]";
            }
            catch
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
        CultureInfo newCulture = culture.ToLowerInvariant() switch
        {
            "fr" => new CultureInfo("fr-FR"),
            "en" => new CultureInfo("en-US"),
            _ => new CultureInfo("en-US")
        };

        if (_currentCulture.TwoLetterISOLanguageName == newCulture.TwoLetterISOLanguageName)
            return;

        _currentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;

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
