using EasySave.ConsoleApp.Config;
using EasyLog;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for AppConfig class, including v2.0 configuration properties.
/// </summary>
public class AppConfigTests : IDisposable
{
    private readonly string _testConfigPath;

    public AppConfigTests()
    {
        // Setup: Clean config for isolated test runs
        _testConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProSoft",
            "EasySave",
            "config.json" // Use actual config file
        );
        
        // CRITICAL: Reset config state between tests
        CleanConfig();
    }
    
    private void CleanConfig()
    {
        // Delete config file to reset to defaults
        if (File.Exists(_testConfigPath))
        {
            // Retry deletion in case of file lock
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    File.Delete(_testConfigPath);
                    break;
                }
                catch (IOException) when (i < 2)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
        }
        
        // Force AppConfig to reload from empty state
        // This clears the singleton cache
        typeof(AppConfig)
            .GetField("_config", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?
            .SetValue(null, null);
    }

    public void Dispose()
    {
        // Cleanup after tests - reset to clean state
        CleanConfig();
    }

    [Fact]
    public void GetLanguage_ShouldReturnDefault_WhenNotConfigured()
    {
        // Act
        string language = AppConfig.GetLanguage();

        // Assert
        Assert.NotNull(language);
        Assert.NotEmpty(language);
    }

    [Fact]
    public void SetLanguage_ShouldPersistValue()
    {
        // Arrange
        string expectedLanguage = "fr";

        // Act
        AppConfig.SetLanguage(expectedLanguage);
        string actualLanguage = AppConfig.GetLanguage();

        // Assert
        Assert.Equal(expectedLanguage, actualLanguage);
    }

    [Fact]
    public void GetLogFormat_ShouldReturnJSON_ByDefault()
    {
        // Act
        LogFormat format = AppConfig.GetLogFormat();

        // Assert
        Assert.Equal(LogFormat.JSON, format);
    }

    [Fact]
    public void SetLogFormat_ShouldPersistValue()
    {
        // Arrange
        LogFormat expectedFormat = LogFormat.XML;

        // Act
        AppConfig.SetLogFormat(expectedFormat);
        LogFormat actualFormat = AppConfig.GetLogFormat();

        // Assert
        Assert.Equal(expectedFormat, actualFormat);
    }

    [Fact]
    public void GetExtensionsToEncrypt_v2_ShouldReturnEmptyList_ByDefault()
    {
        // Act
        List<string> extensions = AppConfig.GetExtensionsToEncrypt();

        // Assert
        Assert.NotNull(extensions);
        Assert.Empty(extensions);
    }

    [Fact]
    public void SetExtensionsToEncrypt_v2_ShouldPersistValue()
    {
        // Arrange
        var expectedExtensions = new List<string> { ".docx", ".xlsx", ".pdf" };

        // Act
        AppConfig.SetExtensionsToEncrypt(expectedExtensions);
        var actualExtensions = AppConfig.GetExtensionsToEncrypt();

        // Assert
        Assert.Equal(expectedExtensions.Count, actualExtensions.Count);
        Assert.Contains(".docx", actualExtensions);
        Assert.Contains(".xlsx", actualExtensions);
        Assert.Contains(".pdf", actualExtensions);
    }

    [Fact]
    public void SetExtensionsToEncrypt_v2_ShouldThrowException_WhenExtensionMissingDot()
    {
        // Arrange
        var invalidExtensions = new List<string> { "docx", ".xlsx" };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            AppConfig.SetExtensionsToEncrypt(invalidExtensions));
        
        Assert.Contains("must start with a dot", exception.Message);
    }

    [Fact]
    public void SetExtensionsToEncrypt_v2_ShouldAllowEmptyList()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        AppConfig.SetExtensionsToEncrypt(emptyList);
        var actualExtensions = AppConfig.GetExtensionsToEncrypt();

        // Assert
        Assert.Empty(actualExtensions);
    }

    [Fact]
    public void GetBusinessSoftwareName_v2_ShouldReturnEmpty_ByDefault()
    {
        // Act
        string softwareName = AppConfig.GetBusinessSoftwareName();

        // Assert
        Assert.NotNull(softwareName);
        Assert.Equal(string.Empty, softwareName);
    }

    [Fact]
    public void SetBusinessSoftwareName_v2_ShouldPersistValue()
    {
        // Arrange
        string expectedName = "calc";

        // Act
        AppConfig.SetBusinessSoftwareName(expectedName);
        string actualName = AppConfig.GetBusinessSoftwareName();

        // Assert
        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void SetBusinessSoftwareName_v2_ShouldAllowEmptyString()
    {
        // Arrange
        AppConfig.SetBusinessSoftwareName("notepad");
        
        // Act - Reset to empty
        AppConfig.SetBusinessSoftwareName(string.Empty);
        string actualName = AppConfig.GetBusinessSoftwareName();

        // Assert
        Assert.Equal(string.Empty, actualName);
    }

    [Fact]
    public void GetMaxLargeFileSize_v3_ShouldReturnZero_ByDefault()
    {
        // Act
        long maxSize = AppConfig.GetMaxLargeFileSize();

        // Assert
        Assert.Equal(0, maxSize);
    }

    [Fact]
    public void SetMaxLargeFileSize_v3_ShouldPersistValue()
    {
        // Arrange
        long expectedSize = 10485760; // 10 MB

        // Act
        AppConfig.SetMaxLargeFileSize(expectedSize);
        long actualSize = AppConfig.GetMaxLargeFileSize();

        // Assert
        Assert.Equal(expectedSize, actualSize);
    }

    [Fact]
    public void SetMaxLargeFileSize_v3_ShouldAllowZero()
    {
        // Arrange
        AppConfig.SetMaxLargeFileSize(1000);
        
        // Act - Reset to 0
        AppConfig.SetMaxLargeFileSize(0);
        long actualSize = AppConfig.GetMaxLargeFileSize();

        // Assert
        Assert.Equal(0, actualSize);
    }

    [Fact]
    public void BackwardCompatibility_v1_ConfigShouldLoadWithNewProperties()
    {
        // Arrange - Simulate v1.0 config (only Language, no v2.0 properties)
        string v1Config = @"{
  ""Language"": ""en"",
  ""LogFormatString"": ""JSON""
}";
        string configDir = Path.GetDirectoryName(_testConfigPath)!;
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
        
        // Write v1.0 config
        File.WriteAllText(_testConfigPath, v1Config);
        
        // Force reload
        CleanConfig();

        // Act - Load config should not throw, and new properties should have defaults
        string language = AppConfig.GetLanguage();
        var extensions = AppConfig.GetExtensionsToEncrypt();
        string businessSoftware = AppConfig.GetBusinessSoftwareName();
        long maxFileSize = AppConfig.GetMaxLargeFileSize();

        // Assert
        Assert.Equal("en", language);
        Assert.Empty(extensions); // Default: empty list
        Assert.Equal(string.Empty, businessSoftware); // Default: empty string
        Assert.Equal(0, maxFileSize); // Default: 0
    }

    [Fact]
    public void MultipleProperties_v2_ShouldAllPersistTogether()
    {
        // Arrange
        var expectedExtensions = new List<string> { ".docx", ".txt" };
        string expectedSoftware = "calc";
        long expectedFileSize = 5242880; // 5 MB

        // Act - Set all v2.0/v3.0 properties
        AppConfig.SetExtensionsToEncrypt(expectedExtensions);
        AppConfig.SetBusinessSoftwareName(expectedSoftware);
        AppConfig.SetMaxLargeFileSize(expectedFileSize);

        // Assert - All should be retrievable
        var actualExtensions = AppConfig.GetExtensionsToEncrypt();
        string actualSoftware = AppConfig.GetBusinessSoftwareName();
        long actualFileSize = AppConfig.GetMaxLargeFileSize();

        Assert.Equal(expectedExtensions.Count, actualExtensions.Count);
        Assert.Equal(expectedSoftware, actualSoftware);
        Assert.Equal(expectedFileSize, actualFileSize);
    }
}
