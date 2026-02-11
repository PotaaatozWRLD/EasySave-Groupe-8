using System.Diagnostics;
using System.Linq;

namespace EasySave.Core.Services;

/// <summary>
/// Service for detecting if specific business software is currently running.
/// Used to prevent backups when critical business applications are active (v2.0).
/// </summary>
public static class BusinessSoftwareDetector
{
    /// <summary>
    /// Checks if a process with the specified name is currently running.
    /// </summary>
    /// <param name="processName">
    /// The name of the process to check (without .exe extension).
    /// For example: "calc" for Calculator, "WINWORD" for Word.
    /// </param>
    /// <returns>
    /// True if at least one instance of the process is running, false otherwise.
    /// Returns false if processName is null or empty.
    /// </returns>
    /// <example>
    /// <code>
    /// bool isRunning = BusinessSoftwareDetector.IsRunning("calc");
    /// if (isRunning)
    /// {
    ///     Console.WriteLine("Calculator is running");
    /// }
    /// </code>
    /// </example>
    public static bool IsRunning(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName))
        {
            return false;
        }

        try
        {
            // Remove .exe extension if provided
            processName = processName.Replace(".exe", "", StringComparison.OrdinalIgnoreCase);
            
            // Check if any process with this name is running
            var processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
        catch (InvalidOperationException)
        {
            // If we can't check (permissions, etc.), assume not running
            return false;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // Win32 error accessing process list, assume not running
            return false;
        }
    }

    /// <summary>
    /// Checks if any process from the specified list is currently running.
    /// </summary>
    /// <param name="processNames">A list of process names to check.</param>
    /// <returns>True if at least one process is running, false otherwise.</returns>
    public static bool IsAnyRunning(IEnumerable<string> processNames)
    {
        if (processNames == null || !processNames.Any())
        {
            return false;
        }

        return processNames.Any(IsRunning);
    }
}
