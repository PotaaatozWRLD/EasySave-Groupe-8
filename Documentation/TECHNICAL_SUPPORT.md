# EasySave 2.0 - Technical Support Documentation

## Software Information

### Product Details

- **Product Name**: EasySave
- **Version**: 2.0
- **Publisher**: ProSoft
- **License Price**: 200 €HT per unit
- **Maintenance Contract**: 12% of purchase price (24 €HT/year)
- **Support Hours**: Monday to Friday, 8:00 AM - 5:00 PM

---

## System Requirements

### Minimum Configuration

- **Operating System**: Windows 10 or later / Windows Server 2016 or later
- **Framework**: .NET Runtime 10.0 or later
- **Processor**: 1 GHz or faster
- **RAM**: 512 MB minimum, 1 GB recommended
- **Disk Space**: 50 MB for application, additional space for logs
- **Permissions**: Read access to source directories, Write access to target directories

### Network Requirements

- Network access if using UNC paths (\\server\share)
- SMB/CIFS protocol support for network drives

---

## Installation

### Default Installation Locations

- **Executable**: `EasySave.Console.exe` (location chosen during installation)
- **Configuration Files**: `%AppData%\ProSoft\EasySave\`
- **Log Files**: `%AppData%\ProSoft\EasySave\Logs\`

### Running the Application

**Method 1: Graphical User Interface (GUI)**

```bash
EasySave.GUI.exe
```

Launches Avalonia-based GUI with:

1. Job list with status indicators
2. Real-time progress monitoring
3. Job editor with validation
4. Settings panel for configuration
5. Encryption and detection controls

**Method 2: Console Interactive Mode**

```bash
EasySave.Console.exe
```

Launches console with:

1. Language selection prompt (English/French)
2. Interactive menu for creating and managing backup jobs
3. Compatible with v1.0/v1.1 console interface

**Method 3: Command Line Mode (Automated)**

```bash
EasySave.Console.exe <job_specification>
```

Executes backup jobs directly without any prompts or user interaction.

**Examples**:

```bash
EasySave.Console.exe 1         # Execute backup job 1 immediately
EasySave.Console.exe 1-3       # Execute jobs 1 through 3 sequentially
EasySave.Console.exe 1;3;5;10  # Execute jobs 1, 3, 5, and 10 (fully automated)
```

**Supported Formats**:

- Single job: `1`
- Range: `1-5` (executes jobs 1, 2, 3, 4, 5)
- List with semicolons: `1;3;5;10` (executes jobs 1, 3, 5, 10)
- Mixed: Not supported (use either range OR list)

### Full Paths (per user)

```
C:\Users\[USERNAME]\AppData\Roaming\ProSoft\EasySave\
├── config.json          (Language settings)
├── jobs.json            (Backup job configurations)
├── state.json           (Real-time backup state)
└── Logs\
    ├── 2026-02-05.json  (Daily log files)
    └── ...
```

---

## Configuration Files

### jobs.json

**Location**: `%AppData%\ProSoft\EasySave\jobs.json`
**Format**: JSON
**Purpose**: Stores all backup job configurations (**unlimited** jobs in v2.0)
**Structure**:

```json
[
  {
    "Name": "Documents Backup",
    "SourcePath": "C:\\Users\\John\\Documents",
    "TargetPath": "D:\\Backups\\Documents",
    "Type": 1,
    "Encryption": {
      "Enabled": true,
      "Extensions": [".docx", ".xlsx", ".pdf"],
      "Algorithm": "AES-256"
    }
  }
]
```

**Type values**:
- `1` = Full backup (all files)
- `2` = Differential backup (changes only)

**Encryption** (NEW v2.0):
- `Enabled`: true/false
- `Extensions`: Array of file extensions to encrypt
- `Algorithm`: Encryption algorithm (currently "AES-256")

### config.json

**Location**: `%AppData%\ProSoft\EasySave\config.json`
**Format**: JSON
**Purpose**: Application settings
**Structure**:

```json
{
  "Language": "en",
  "LogFormatString": "JSON",
  "BusinessSoftwareDetectionEnabled": true,
  "BusinessSoftwareList": [
    "WINWORD.EXE",
    "EXCEL.EXE",
    "POWERPNT.EXE",
    "sqlservr.exe",
    "MYSQLD.EXE"
  ]
}
```

**Fields**:
- `Language`: "en" (English) or "fr" (French)
- `LogFormatString`: "JSON" or "XML"
- `BusinessSoftwareDetectionEnabled`: true/false (NEW v2.0)
- `BusinessSoftwareList`: Array of executable names to monitor (NEW v2.0)

### state.json / state.xml

**Location**: `%AppData%\ProSoft\EasySave\state.json` OR `state.xml` (v1.1)
**Format**: JSON or XML (depends on selected log format)
**Purpose**: Real-time state of active backup jobs
**Updated**: During backup execution

**JSON Structure**:

```json
{
  "Name": "Documents Backup",
  "LastActionTimestamp": "2026-02-09 14:30:25",
  "State": 0,  // 0 = ACTIVE, 1 = END, 2 = PAUSED
  "TotalFiles": 1250,
  "TotalSize": 52428800,
  "Progression": 45,
  "NbFilesLeftToDo": 687,
  "NbFilesLeftToDoSize": 28835840,
  "CurrentSourceFilePath": "\\\\localhost\\C$\\Users\\John\\Documents\\file.txt",
  "CurrentTargetFilePath": "\\\\localhost\\D$\\Backups\\Documents\\file.txt"
}
```

**XML Structure (v1.1)**:

```xml
<?xml version="1.0" encoding="utf-8"?>
<StateEntries>
  <StateEntry>
    <Name>Documents Backup</Name>
    <LastActionTimestamp>2026-02-09T14:30:25</LastActionTimestamp>
    <State>ACTIVE</State>
    <TotalFiles>1250</TotalFiles>
    <TotalSize>52428800</TotalSize>
    <Progression>45</Progression>
    <NbFilesLeftToDo>687</NbFilesLeftToDo>
    <NbFilesLeftToDoSize>28835840</NbFilesLeftToDoSize>
    <CurrentSourceFilePath>\\localhost\C$\Users\John\Documents\file.txt</CurrentSourceFilePath>
    <CurrentTargetFilePath>\\localhost\D$\Backups\Documents\file.txt</CurrentTargetFilePath>
  </StateEntry>
</StateEntries>
```

### Daily Log Files

**Location**: `%AppData%\ProSoft\EasySave\Logs\YYYY-MM-DD.json` OR `.xml`
**Format**: JSON or XML (depends on selected log format)
**Purpose**: Detailed log of all file operations
**Retention**: Not automatically deleted (manual cleanup required)

**Log Entry Structure (v2.0 with Encryption)**:

```json
{
  "Timestamp": "2026-02-11T14:30:25",
  "JobName": "Documents Backup",
  "SourcePath": "\\\\localhost\\C$\\Documents\\file.docx",
  "TargetPath": "\\\\localhost\\D$\\Backup\\file.docx",
  "FileName": "file.docx",
  "FileSize": 2048000,
  "TransferTime": 250,
  "EncryptionTime": 125,
  "ErrorMessage": null
}
```

**NEW in v2.0**: `EncryptionTime` field
- `0`: File not encrypted (extension not selected)
- `> 0`: Encryption completed successfully (milliseconds)
- `< 0`: Encryption failed (file backed up unencrypted)

---

## Command Line Arguments

### Syntax

```
EasySave.exe [job_specification]
```

### Examples

- `EasySave.exe` - Launch interactive menu
- `EasySave.exe 1` - Run job number 1
- `EasySave.exe 1-3` - Run jobs 1 through 3
- `EasySave.exe 1;3;5` - Run jobs 1, 3, and 5

---

## Troubleshooting

### Common Issues

#### Application Won't Start

- Verify .NET 10.0 Runtime is installed
- Check Windows Event Viewer for errors
- Ensure user has AppData access

#### Backup Fails with "Access Denied"

- Verify source folder read permissions
- Verify target folder write permissions
- Check if antivirus is blocking file access
- For network paths, verify network credentials

#### "Cannot add more than 5 jobs" (v1.0/v1.1 only)

- **This limitation is REMOVED in v2.0**
- v2.0 supports unlimited backup jobs
- No need to delete old jobs to add new ones

#### Log Files Not Created

- Check AppData folder permissions
- Verify disk space availability
- Check `%AppData%\ProSoft\EasySave\Logs\` exists
- **v1.1:** Verify log format is configured correctly (Settings menu, Option 9)

#### Business Software Detected (NEW v2.0)

**Error**: "Business software detected. Backup blocked."

- A configured application (Word, Excel, SQL Server, etc.) is running
- Close the application and retry
- Or disable detection temporarily in Settings
- Next backup will execute normally

**Solution**:
1. Close the detected application
2. Wait 5-10 seconds
3. Retry backup
4. Check Settings if you want to disable detection

#### Encryption Failed (NEW v2.0)

**Symptom**: `EncryptionTime` is negative in logs

- File was backed up but encryption failed
- Check disk space availability
- Verify file is not locked by another process
- Check encryption algorithm compatibility

**Solution**:
1. Verify target drive has sufficient space
2. Close any applications using the target directory
3. Retry the backup
4. Check logs for specific error details

#### Wrong Log Format

- Open Settings (GUI or Option 9 in console)
- Check current log format displayed
- Change to desired format (JSON or XML)
- Next backup will use new format
- Previous logs remain in original format

#### XML Parsing Errors

- XML files require proper structure
- Manual editing of XML logs not recommended
- If corrupted, delete problematic .xml file
- Logger will create new file on next backup

### Log Analysis

- **TransferTime > 0**: Successful transfer (time in milliseconds)
- **TransferTime = 0**: File not copied (differential backup, file unchanged)
- **TransferTime < 0**: Error occurred during transfer
- **ErrorMessage != null**: Details about the error

---

## Technical Architecture

### Components

1. **EasySave.GUI.exe**: Graphical user interface (Avalonia/MVVM) - **NEW v2.0**
2. **EasySave.Console.exe**: Console application (CLI)
3. **EasyLog.dll**: Logging library (JSON/XML)
4. **EasySave.Core.dll**: Business logic (encryption, detection, backup)
5. **EasySave.Shared.dll**: Shared models and configuration

### Backup Types

- **Full Backup**: Copies all files regardless of modification date
- **Differential Backup**: Only copies files newer than target (based on LastWriteTime)

### Path Format

- All paths are converted to UNC format in logs
- Local paths (C:\) become `\\localhost\C$\`
- Network paths remain unchanged

---

## Support Escalation

### Level 1 Support

- User education on features
- Configuration assistance
- Basic troubleshooting

### Level 2 Support (Contact Development Team)

- Application crashes
- Data corruption issues
- Performance problems
- Feature requests

### Contact Information

- **Support Email**: <support@prosoft.com>
- **Technical Hotline**: Available during support hours (8-17h, Mon-Fri)
- **GitHub Issues**: For bug reports and feature requests

---

## Backup Recommendations

### Best Practices

1. Test backup jobs with small datasets first
2. Use differential backups for frequent backups
3. Use full backups weekly or monthly
4. Verify target drive has sufficient space
5. Monitor log files for recurring errors
6. Keep target drives on separate physical hardware

### Performance Considerations

- Large files (>1GB) may take significant time
- Network backups are slower than local
- Antivirus scanning can slow file copies
- Consider backup schedules during off-hours

## Encryption Technical Details

### AES-256 Implementation

- **Algorithm**: AES (Advanced Encryption Standard) with 256-bit key
- **Mode**: CBC (Cipher Block Chaining) with PKCS7 padding
- **Performance**: ~10-20% overhead depending on file size
- **Security**: Military-grade encryption suitable for confidential data

### Business Software Detection

- **Method**: Process name matching via Windows task manager
- **Frequency**: Checked before job execution
- **Thread-safe**: Detection runs on separate thread
- **False positives**: Minimized through exact executable name matching

---

**Document Version**: 2.0
**Last Updated**: February 11, 2026
**ProSoft Technical Support Team**
