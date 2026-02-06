using EasySave.Core.Helpers;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for PathHelper class.
/// Tests UNC path conversion for local and network paths.
/// </summary>
public class PathHelperTests
{
    [Fact]
    public void ToUncPath_ShouldConvertLocalDriveC()
    {
        // Arrange
        string localPath = @"C:\Documents\File.txt";

        // Act
        string uncPath = PathHelper.ToUncPath(localPath);

        // Assert
        Assert.Equal(@"\\localhost\C$\Documents\File.txt", uncPath);
    }

    [Fact]
    public void ToUncPath_ShouldConvertLocalDriveD()
    {
        // Arrange
        string localPath = @"D:\Backups\Data";

        // Act
        string uncPath = PathHelper.ToUncPath(localPath);

        // Assert
        Assert.Equal(@"\\localhost\D$\Backups\Data", uncPath);
    }

    [Fact]
    public void ToUncPath_ShouldPreserveAlreadyUNCPath()
    {
        // Arrange
        string uncPath = @"\\server\share\folder\file.txt";

        // Act
        string result = PathHelper.ToUncPath(uncPath);

        // Assert
        Assert.Equal(@"\\server\share\folder\file.txt", result);
    }

    [Fact]
    public void ToUncPath_ShouldPreserveNetworkPath()
    {
        // Arrange
        string networkPath = @"\\192.168.1.100\SharedFolder\Document.docx";

        // Act
        string result = PathHelper.ToUncPath(networkPath);

        // Assert
        Assert.Equal(@"\\192.168.1.100\SharedFolder\Document.docx", result);
    }

    [Fact]
    public void ToUncPath_ShouldHandleEmptyString()
    {
        // Arrange
        string emptyPath = "";

        // Act
        string result = PathHelper.ToUncPath(emptyPath);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ToUncPath_ShouldHandleNullString()
    {
        // Arrange
        string? nullPath = null;

        // Act
        string? result = PathHelper.ToUncPath(nullPath!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToUncPath_ShouldConvertRootDrive()
    {
        // Arrange
        string rootPath = @"E:\";

        // Act
        string uncPath = PathHelper.ToUncPath(rootPath);

        // Assert
        Assert.Equal(@"\\localhost\E$\", uncPath);
    }

    [Fact]
    public void ToUncPath_ShouldHandleDeepNestedPath()
    {
        // Arrange
        string deepPath = @"C:\Users\Admin\Documents\Projects\EasySave\Data\Logs\2026\February\file.log";

        // Act
        string uncPath = PathHelper.ToUncPath(deepPath);

        // Assert
        Assert.Equal(@"\\localhost\C$\Users\Admin\Documents\Projects\EasySave\Data\Logs\2026\February\file.log", uncPath);
    }
}
