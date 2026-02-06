using EasySave.ConsoleApp.Languages;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for LanguageManager class.
/// Tests multi-language support (EN/FR) and singleton pattern.
/// </summary>
public class LanguageManagerTests
{
    [Fact]
    public void LanguageManager_ShouldBeSingleton()
    {
        // Act
        var instance1 = LanguageManager.Instance;
        var instance2 = LanguageManager.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void GetString_ShouldReturnValueForValidKey()
    {
        // Arrange
        var manager = LanguageManager.Instance;

        // Act
        string welcome = manager.GetString("Welcome");

        // Assert
        Assert.NotNull(welcome);
        Assert.NotEmpty(welcome);
        Assert.DoesNotContain("[Welcome]", welcome); // Should not be placeholder
    }

    [Fact]
    public void GetString_ShouldReturnPlaceholderForInvalidKey()
    {
        // Arrange
        var manager = LanguageManager.Instance;

        // Act
        string result = manager.GetString("NonExistentKey12345");

        // Assert
        Assert.Equal("[NonExistentKey12345]", result);
    }

    [Fact]
    public void CurrentLanguage_ShouldReturnValidLanguageCode()
    {
        // Arrange
        var manager = LanguageManager.Instance;

        // Act
        string currentLang = manager.CurrentLanguage;

        // Assert
        Assert.True(currentLang == "en" || currentLang == "fr", 
            $"Language should be 'en' or 'fr', but was '{currentLang}'");
    }

    [Fact]
    public void ToggleLanguage_ShouldSwitchBetweenEnglishAndFrench()
    {
        // Arrange
        var manager = LanguageManager.Instance;
        string initialLang = manager.CurrentLanguage;

        // Act
        manager.ToggleLanguage();
        string newLang = manager.CurrentLanguage;

        // Assert
        Assert.NotEqual(initialLang, newLang);
        Assert.True((initialLang == "en" && newLang == "fr") || 
                   (initialLang == "fr" && newLang == "en"));

        // Cleanup - toggle back to original
        manager.ToggleLanguage();
    }

    [Fact]
    public void ChangeLanguage_ShouldLoadCorrectLanguageStrings()
    {
        // Arrange
        var manager = LanguageManager.Instance;

        // Act - Switch to French
        manager.ChangeLanguage("fr");
        string frenchWelcome = manager.GetString("Welcome");

        // Switch to English
        manager.ChangeLanguage("en");
        string englishWelcome = manager.GetString("Welcome");

        // Assert
        Assert.NotEqual(frenchWelcome, englishWelcome);
        Assert.Contains("Bienvenue", frenchWelcome);
        Assert.Contains("Welcome", englishWelcome);
    }

    [Fact]
    public void GetString_ShouldReturnAllMenuKeys()
    {
        // Arrange
        var manager = LanguageManager.Instance;
        string[] requiredKeys = new[] 
        { 
            "Welcome", "Menu_List", "Menu_Create", "Menu_Edit", 
            "Menu_Delete", "Menu_Run", "Menu_RunAll", "Menu_Logs", 
            "Menu_Lang", "Menu_Exit" 
        };

        // Act & Assert
        foreach (var key in requiredKeys)
        {
            string value = manager.GetString(key);
            Assert.NotNull(value);
            Assert.DoesNotContain($"[{key}]", value);
        }
    }
}
