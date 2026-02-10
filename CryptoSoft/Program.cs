using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSoft;

/// <summary>
/// CryptoSoft - External encryption software for EasySave v2.0.
/// Encrypts files using XOR cipher with a simple key.
/// 
/// Usage: CryptoSoft.exe &lt;sourceFile&gt; &lt;targetFile&gt;
/// Exit codes:
///   0: Success
///  -1: Invalid arguments
///  -2: Source file not found
///  -3: Encryption error
///  -4: Write error
/// </summary>
class Program
{
    private const string EncryptionKey = "ProSoft_EasySave_SecureKey_2026"; // Simple demo key

    static int Main(string[] args)
    {
        try
        {
            // Validate arguments
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: CryptoSoft.exe <sourceFile> <targetFile>");
                Console.Error.WriteLine("Error: Invalid number of arguments (expected 2, got {0})", args.Length);
                return -1;
            }

            string sourceFile = args[0];
            string targetFile = args[1];

            // Validate source file exists
            if (!File.Exists(sourceFile))
            {
                Console.Error.WriteLine($"Error: Source file not found: {sourceFile}");
                return -2;
            }

            // Log start
            Console.WriteLine($"CryptoSoft v2.0 - Starting encryption");
            Console.WriteLine($"Source: {sourceFile}");
            Console.WriteLine($"Target: {targetFile}");

            var stopwatch = Stopwatch.StartNew();

            // Read source file
            byte[] sourceData;
            try
            {
                sourceData = File.ReadAllBytes(sourceFile);
                Console.WriteLine($"Read {sourceData.Length} bytes from source");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading source file: {ex.Message}");
                return -3;
            }

            // Encrypt data (XOR cipher with key)
            byte[] encryptedData;
            try
            {
                encryptedData = XorEncrypt(sourceData, EncryptionKey);
                Console.WriteLine($"Encrypted {encryptedData.Length} bytes");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during encryption: {ex.Message}");
                return -3;
            }

            // Ensure target directory exists
            string? targetDir = Path.GetDirectoryName(targetFile);
            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
            {
                try
                {
                    Directory.CreateDirectory(targetDir);
                    Console.WriteLine($"Created target directory: {targetDir}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error creating target directory: {ex.Message}");
                    return -4;
                }
            }

            // Write encrypted data to target
            try
            {
                File.WriteAllBytes(targetFile, encryptedData);
                Console.WriteLine($"Wrote {encryptedData.Length} bytes to target");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing target file: {ex.Message}");
                return -4;
            }

            stopwatch.Stop();
            Console.WriteLine($"Encryption completed successfully in {stopwatch.ElapsedMilliseconds} ms");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return -3;
        }
    }

    /// <summary>
    /// XOR encryption/decryption using a repeating key.
    /// Simple cipher for demonstration purposes (v2.0).
    /// Note: XOR with same key both encrypts and decrypts.
    /// </summary>
    /// <param name="data">Data to encrypt/decrypt</param>
    /// <param name="key">Encryption key (will be repeated if shorter than data)</param>
    /// <returns>Encrypted/decrypted data</returns>
    private static byte[] XorEncrypt(byte[] data, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] result = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return result;
    }
}
