using System.Text.RegularExpressions;

namespace EasySave.Core.Helpers;

/// <summary>
/// Helper class for path manipulation and UNC format conversion.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Converts a local or network path to UNC (Universal Naming Convention) format.
    /// Examples:
    ///   C:\Folder\File.txt -> \\localhost\C$\Folder\File.txt
    ///   D:\Data\Backup -> \\localhost\D$\Data\Backup
    ///   \\server\share\file.txt -> \\server\share\file.txt (already UNC)
    /// </summary>
    /// <param name="path">The path to convert</param>
    /// <returns>The path in UNC format</returns>
    public static string ToUncPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return path;

        // Already in UNC format (starts with \\)
        if (path.StartsWith(@"\\"))
            return path;

        // Check if it's a local path with drive letter (e.g., C:\...)
        if (Regex.IsMatch(path, @"^[a-zA-Z]:\\"))
        {
            // Convert C:\ to \\localhost\C$\
            char driveLetter = path[0];
            string remainingPath = path.Substring(3); // Skip "C:\"
            return $@"\\localhost\{driveLetter}$\{remainingPath}";
        }

        // If it's a relative path or other format, return as-is
        return path;
    }
}
