# CryptoSoft v2.0

External encryption software for **EasySave v2.0** (Prosit 4 - Livrable 2).

## Overview

CryptoSoft is a standalone command-line encryption tool used by EasySave to encrypt backup files based on configured extensions.

## Features (v2.0)

- ✅ XOR cipher encryption (simple, fast, reversible)
- ✅ Command-line interface
- ✅ Exit codes for error handling
- ✅ Console output for debugging
- ✅ Creates target directories automatically
- ❌ **NOT** mono-instance (v3.0 feature)

## Usage

```bash
CryptoSoft.exe <sourceFile> <targetFile>
```

### Examples

```bash
# Encrypt a single file
CryptoSoft.exe "C:\Data\document.docx" "C:\Backup\document.docx.encrypted"

# Decrypt (same command - XOR is reversible)
CryptoSoft.exe "C:\Backup\document.docx.encrypted" "C:\Data\document.docx"
```

## Exit Codes

| Code | Meaning |
|------|---------|
| `0`  | Success |
| `-1` | Invalid arguments (wrong number of parameters) |
| `-2` | Source file not found |
| `-3` | Encryption error (read/encrypt failure) |
| `-4` | Write error (target file/directory) |

## Integration with EasySave

EasySave v2.0 calls CryptoSoft as an external process:

```csharp
var process = Process.Start(new ProcessStartInfo
{
    FileName = "CryptoSoft.exe",
    Arguments = $"\"{sourceFile}\" \"{targetFile}\"",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true
});

process.WaitForExit();
int exitCode = process.ExitCode;
```

## Encryption Algorithm

**v2.0:** XOR cipher with fixed key `ProSoft_EasySave_SecureKey_2026`

- Simple and fast for demonstration
- Reversible (encrypt = decrypt with same key)
- **NOT secure for production** (educational purpose only)

## Build

```bash
cd CryptoSoft
dotnet build -c Release
```

Output: `CryptoSoft\bin\Release\net10.0\CryptoSoft.exe`

## Test

```bash
# Create test file
echo "Hello EasySave" > test.txt

# Encrypt
dotnet run --project CryptoSoft\CryptoSoft.csproj test.txt test.encrypted

# Decrypt
dotnet run --project CryptoSoft\CryptoSoft.csproj test.encrypted test.decrypted

# Compare
type test.decrypted
```

## Version History

### v2.0 (Livrable 2)

- Initial release
- XOR cipher encryption
- Command-line interface
- Exit codes for error handling

### v3.0 (Livrable 3 - Planned)

- **Mono-instance** using Mutex
- Prevent simultaneous executions
- Queue management for concurrent requests

## Notes

⚠️ **CryptoSoft is EXTERNAL to EasySave solution**

- Separate repository location
- Independent build/deployment
- EasySave calls it as external process

⚠️ **Prosit 4 Context**

- This software is developed during Prosit 4
- Integrated with EasySave in Livrable 2
- Modified for mono-instance in v3.0 (Livrable 3)

## License

ProSoft © 2026 - Internal tool for EasySave project
