using Xunit;
using EasySave.Core.Services;
using System.Diagnostics;

namespace EasySave.Tests;

/// <summary>
/// Tests for EncryptionService (v2.0).
/// Validates CryptoSoft.exe integration, file encryption, and error handling.
/// </summary>
public class EncryptionServiceTests : IDisposable
{
    private readonly string _testDir;
    private readonly string _cryptoSoftPath;

    public EncryptionServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "EasySaveEncryptionTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);

        // Use the CryptoSoft.exe from the build output
        string workspaceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        _cryptoSoftPath = Path.Combine(workspaceRoot, "CryptoSoft", "bin", "Release", "net10.0", "CryptoSoft.exe");
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, true);
            }
        }
        catch { /* Ignore cleanup errors */ }
    }

    [Fact]
    public void EncryptFile_WithValidFile_ShouldReturnPositiveTime()
    {
        // Skip if CryptoSoft.exe not built
        if (!File.Exists(_cryptoSoftPath))
        {
            return; // Skip test if CryptoSoft not available
        }

        // Arrange
        string sourceFile = Path.Combine(_testDir, "test.txt");
        string targetFile = Path.Combine(_testDir, "test.encrypted");
        File.WriteAllText(sourceFile, "Test content for encryption");

        var service = new EncryptionService(_cryptoSoftPath);

        // Act
        long encryptionTime = service.EncryptFile(sourceFile, targetFile);

        // Assert
        Assert.True(encryptionTime > 0, $"Encryption should succeed with positive time, got {encryptionTime}");
        Assert.True(File.Exists(targetFile), "Encrypted file should exist");
        Assert.True(new FileInfo(targetFile).Length > 0, "Encrypted file should not be empty");
    }

    [Fact]
    public void EncryptFile_WithNonExistentSource_ShouldReturnNegativeTwo()
    {
        // Skip if CryptoSoft.exe not built
        if (!File.Exists(_cryptoSoftPath))
        {
            return;
        }

        // Arrange
        string sourceFile = Path.Combine(_testDir, "nonexistent.txt");
        string targetFile = Path.Combine(_testDir, "output.encrypted");
        var service = new EncryptionService(_cryptoSoftPath);

        // Act
        long result = service.EncryptFile(sourceFile, targetFile);

        // Assert
        Assert.Equal(-2, result); // -2 = source file not found
    }

    [Fact]
    public void EncryptFile_WithInvalidCryptoSoftPath_ShouldReturnNegativeOne()
    {
        // Arrange
        string sourceFile = Path.Combine(_testDir, "test.txt");
        string targetFile = Path.Combine(_testDir, "test.encrypted");
        File.WriteAllText(sourceFile, "Test content");

        var service = new EncryptionService("C:\\NonExistent\\CryptoSoft.exe");

        // Act
        long result = service.EncryptFile(sourceFile, targetFile);

        // Assert
        Assert.Equal(-1, result); // -1 = CryptoSoft.exe not found
    }

    [Fact]
    public void ShouldEncrypt_WithMatchingExtension_ShouldReturnTrue()
    {
        // Arrange
        string filePath = "C:\\Documents\\report.docx";
        var extensions = new List<string> { ".docx", ".xlsx", ".pdf" };

        // Act
        bool result = EncryptionService.ShouldEncrypt(filePath, extensions);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldEncrypt_WithNonMatchingExtension_ShouldReturnFalse()
    {
        // Arrange
        string filePath = "C:\\Documents\\image.png";
        var extensions = new List<string> { ".docx", ".xlsx", ".pdf" };

        // Act
        bool result = EncryptionService.ShouldEncrypt(filePath, extensions);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldEncrypt_WithEmptyExtensionList_ShouldReturnFalse()
    {
        // Arrange
        string filePath = "C:\\Documents\\report.docx";
        var extensions = new List<string>();

        // Act
        bool result = EncryptionService.ShouldEncrypt(filePath, extensions);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldEncrypt_CaseInsensitive_ShouldMatch()
    {
        // Arrange
        string filePath = "C:\\Documents\\Report.DOCX";
        var extensions = new List<string> { ".docx", ".xlsx" };

        // Act
        bool result = EncryptionService.ShouldEncrypt(filePath, extensions);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EncryptFile_AndDecrypt_ShouldRecoverOriginal()
    {
        // Skip if CryptoSoft.exe not built
        if (!File.Exists(_cryptoSoftPath))
        {
            return;
        }

        // Arrange
        string originalContent = "ProSoft EasySave v2.0 encryption test";
        string sourceFile = Path.Combine(_testDir, "original.txt");
        string encryptedFile = Path.Combine(_testDir, "encrypted.txt");
        string decryptedFile = Path.Combine(_testDir, "decrypted.txt");
        
        File.WriteAllText(sourceFile, originalContent);
        var service = new EncryptionService(_cryptoSoftPath);

        // Act - Encrypt
        long encryptTime = service.EncryptFile(sourceFile, encryptedFile);
        Assert.True(encryptTime > 0, "Encryption should succeed");

        // Act - Decrypt (XOR is reversible)
        long decryptTime = service.EncryptFile(encryptedFile, decryptedFile);
        Assert.True(decryptTime > 0, "Decryption should succeed");

        // Assert
        string decryptedContent = File.ReadAllText(decryptedFile);
        Assert.Equal(originalContent, decryptedContent);
    }
}
