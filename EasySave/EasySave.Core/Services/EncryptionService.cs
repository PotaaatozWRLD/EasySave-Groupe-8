using System.Diagnostics;

namespace EasySave.Core.Services;

/// <summary>
/// Service for encrypting files using the external CryptoSoft application (v2.0).
/// Launches CryptoSoft.exe as a separate process and measures encryption time.
/// </summary>
public class EncryptionService
{
    private readonly string _cryptoSoftPath;
    private readonly int _timeoutMilliseconds;

    /// <summary>
    /// Initializes a new instance of EncryptionService.
    /// </summary>
    /// <param name="cryptoSoftPath">Path to CryptoSoft.exe executable.</param>
    /// <param name="timeoutMilliseconds">Maximum time to wait for encryption (default: 60 seconds).</param>
    public EncryptionService(string cryptoSoftPath, int timeoutMilliseconds = 60000)
    {
        _cryptoSoftPath = cryptoSoftPath;
        _timeoutMilliseconds = timeoutMilliseconds;
    }

    /// <summary>
    /// Encrypts a file using CryptoSoft.exe.
    /// </summary>
    /// <param name="sourceFile">Path to the file to encrypt.</param>
    /// <param name="targetFile">Path where encrypted file will be saved.</param>
    /// <returns>
    /// Encryption time in milliseconds if successful (>0),
    /// or negative error code if failed:
    ///   -1: CryptoSoft.exe not found
    ///   -2: Source file not found
    ///   -3: CryptoSoft returned error exit code
    ///   -4: Timeout waiting for CryptoSoft
    ///   -5: Exception during encryption
    /// </returns>
    public long EncryptFile(string sourceFile, string targetFile)
    {
        try
        {
            // Validate CryptoSoft.exe exists
            if (!File.Exists(_cryptoSoftPath))
            {
                return -1; // CryptoSoft not found
            }

            // Validate source file exists
            if (!File.Exists(sourceFile))
            {
                return -2; // Source file not found
            }

            var stopwatch = Stopwatch.StartNew();

            // Launch CryptoSoft.exe as external process
            var processInfo = new ProcessStartInfo
            {
                FileName = _cryptoSoftPath,
                Arguments = $"\"{sourceFile}\" \"{targetFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                return -5; // Failed to start process
            }

            // Wait for completion with timeout
            bool finished = process.WaitForExit(_timeoutMilliseconds);
            stopwatch.Stop();

            if (!finished)
            {
                process.Kill();
                return -4; // Timeout
            }

            // Check exit code
            if (process.ExitCode != 0)
            {
                return -3; // CryptoSoft error
            }

            // Verify target file was created
            if (!File.Exists(targetFile))
            {
                return -3; // Output file not created
            }

            return stopwatch.ElapsedMilliseconds;
        }
        catch (InvalidOperationException)
        {
            return -5; // Process error
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return -5; // Win32 error (process not found, etc.)
        }
        catch (IOException)
        {
            return -5; // File I/O error
        }
    }

    /// <summary>
    /// Checks if a file extension should be encrypted based on the configured list.
    /// </summary>
    /// <param name="filePath">File path to check.</param>
    /// <param name="extensionsToEncrypt">List of extensions to encrypt (e.g., ".docx", ".xlsx").</param>
    /// <returns>True if the file should be encrypted, false otherwise.</returns>
    public static bool ShouldEncrypt(string filePath, List<string> extensionsToEncrypt)
    {
        if (extensionsToEncrypt == null || extensionsToEncrypt.Count == 0)
        {
            return false;
        }

        string extension = Path.GetExtension(filePath);
        return extensionsToEncrypt.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}
