using System;

namespace EasySave.GUI.Services;

/// <summary>
/// Service for managing application language/localization.
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Gets the current language code (e.g., "en", "fr").
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// Gets a localized string by key.
    /// </summary>
    /// <param name="key">Localization key.</param>
    /// <returns>Localized string or key if not found.</returns>
    string GetString(string key);

    /// <summary>
    /// Changes the current application language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "fr").</param>
    void ChangeLanguage(string languageCode);

    /// <summary>
    /// Event raised when language changes.
    /// </summary>
    event EventHandler? LanguageChanged;
}
