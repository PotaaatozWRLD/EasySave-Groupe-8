# EasySave 3.0 - Technical Support Documentation

## Software Information

### Product Details

- **Product Name**: EasySave
- **Version**: 3.0
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
  ],
  "ParallelExecutionEnabled": true,
  "LargeFileThresholdKB": 5000,
  "DetectionInterval": 5000,
  "PriorityExtensions": [
    ".docx",
    ".xlsx",
    ".pptx",
    ".sql"
  ],
  "LoggingConfiguration": {
    "LocalLogsEnabled": true,
    "CentralizedLogsEnabled": false,
    "DockerServiceUrl": "http://localhost:8080",
    "SyncInterval": 5000,
    "UseLocalFallback": true
  },
  "CryptoSoftSettings": {
    "MonoInstanceEnabled": true,
    "LockFilePath": "%AppData%\\ProSoft\\CryptoSoft\\",
    "InstanceTimeout": 30000
  }
}
```

**Key Fields (v3.0)**:
- `Language`: "en" (English) or "fr" (French)
- `LogFormatString`: "JSON" or "XML"
- `BusinessSoftwareDetectionEnabled`: true/false (auto-pause on detection)
- `BusinessSoftwareList`: Array of executable names to monitor
- `ParallelExecutionEnabled`: true/false (multiple jobs simultaneously)
- `LargeFileThresholdKB`: Size in KB above which exclusive transfer applies
- `DetectionInterval`: Milliseconds between software checks
- `PriorityExtensions`: Array of file extensions treated as high-priority
- `LoggingConfiguration`: Docker centralized logging settings
- `CryptoSoftSettings`: Mono-instance mutex enforcement

### state.json / state.xml

**Location**: `%AppData%\ProSoft\EasySave\state.json` OR `state.xml` (v1.1)
**Format**: JSON or XML (depends on selected log format)
**Purpose**: Real-time state of active backup jobs
**Updated**: During backup execution

**JSON Structure (v3.0 with Pause/Resume)**:

```json
[
  {
    "Name": "Documents Backup",
    "LastActionTimestamp": "2026-02-25 14:30:25",
    "State": 0,
    "TotalFiles": 1250,
    "TotalSize": 52428800,
    "Progression": 45,
    "NbFilesLeftToDo": 687,
    "NbFilesLeftToDoSize": 28835840,
    "CurrentSourceFilePath": "\\\\localhost\\C$\\Users\\John\\Documents\\file.txt",
    "CurrentTargetFilePath": "\\\\localhost\\D$\\Backups\\Documents\\file.txt",
    "IsPaused": false,
    "IsRunning": true,
    "PausedAt": null
  }
]
```

**State Values**:
- `0` = ACTIVE
- `1` = END (completed)
- `2` = PAUSED (user-initiated or auto-paused)

**NEW Fields (v3.0)**:
- `IsPaused`: true if job is paused by user or business software detection
- `IsRunning`: true if actively transferring files
- `PausedAt`: timestamp when pause occurred (for checkpoint restore)

### Daily Log Files

**Location**: `%AppData%\ProSoft\EasySave\Logs\YYYY-MM-DD.json` OR `.xml`
**Format**: JSON or XML (depends on selected log format)
**Purpose**: Detailed log of all file operations
**Retention**: Not automatically deleted (manual cleanup required)

**Log Entry Structure (v3.0 with User Identification)**:

```json
[
  {
    "UserName": "john.doe",
    "ComputerName": "WORKSTATION-01",
    "Timestamp": "2026-02-25T14:30:25",
    "JobName": "Documents Backup",
    "SourcePath": "\\\\WORKSTATION-01\\C$\\Users\\john.doe\\Documents\\file.docx",
    "TargetPath": "\\\\SERVER-BACKUP\\Backups\\Documents\\file.docx",
    "FileName": "file.docx",
    "FileSize": 2048000,
    "TransferTime": 250,
    "EncryptionTime": 125,
    "ErrorMessage": null
  }
]
```

**Fields (v3.0)**:
- `UserName`: Windows username (NEW v3.0 for centralized logging)
- `ComputerName`: Computer name (NEW v3.0 for centralized logging)
- `Timestamp`: ISO 8601 format
- `JobName`: Backup job identifier
- `SourcePath`: UNC format path
- `TargetPath`: UNC format path
- `FileName`: Name of file transferred
- `FileSize`: Bytes transferred
- `TransferTime`: Milliseconds, negative if error
- `EncryptionTime`: Milliseconds, 0 if not encrypted, negative if failed
- `ErrorMessage`: Error details if applicable

### Centralized Log Service (NEW v3.0)

**Location**: Docker service (if enabled)
**Path**: `/var/logs/easysave/YYYY-MM-DD.json`
**Format**: JSON (aggregated from all users/machines)

**Central Log Entry** (combines all client logs):

```json
{
  "UserName": "john.doe",
  "ComputerName": "WORKSTATION-01",
  "Timestamp": "2026-02-25T14:30:25",
  "JobName": "Documents Backup",
  "SourcePath": "\\\\WORKSTATION-01\\C$\\Users\\john.doe\\Documents\\file.docx",
  "TargetPath": "\\\\SERVER-BACKUP\\Backups\\Documents\\file.docx",
  "FileName": "file.docx",
  "FileSize": 2048000,
  "TransferTime": 250,
  "EncryptionTime": 125,
  "ErrorMessage": null
}
```

#### Docker Log Service API

**REST Endpoints** (for centralized logging):

```bash
# Health check
GET /api/health
Response: {"status":"healthy","version":"3.0"}

# Retrieve logs for specific date
GET /api/logs/2026-02-25
Response: [LogEntry, LogEntry, ...]

# Retrieve logs for specific user
GET /api/logs?user=john.doe
Response: [LogEntry, ...]

# Retrieve logs for specific computer
GET /api/logs?computer=WORKSTATION-01
Response: [LogEntry, ...]

# Retrieve logs for specific job
GET /api/logs?job=Documents%20Backup
Response: [LogEntry, ...]

# Real-time log streaming
WebSocket /api/logs/stream
```

#### Docker Compose Configuration

**File**: `docker-compose.yml`

```yaml
version: '3.8'

services:
  easysave-log-service:
    image: prosoft/easysave-log-service:3.0
    container_name: easysave-logs
    ports:
      - "8080:8080"
    environment:
      - LOG_STORAGE_PATH=/var/logs/easysave
      - DATABASE_URL=postgres://logs:password@postgres:5432/easysave
      - LOG_RETENTION_DAYS=365
    volumes:
      - ./logs:/var/logs/easysave
      - ./data/postgresql:/var/lib/postgresql/data
    depends_on:
      - postgres
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/api/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  postgres:
    image: postgres:15-alpine
    container_name: easysave-postgres
    environment:
      - POSTGRES_USER=logs
      - POSTGRES_PASSWORD=password
      - POSTGRES_DB=easysave
    volumes:
      - ./data/postgresql:/var/lib/postgresql/data
    restart: unless-stopped
```

**Deployment**:

```bash
# Deploy services
docker-compose up -d

# Verify services running
docker-compose ps

# View service logs
docker-compose logs easysave-log-service

# Stop services
docker-compose down
```

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

### Encryption Failed (NEW v2.0)

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

### Wrong Log Format

- Open Settings (GUI or Option 9 in console)
- Check current log format displayed
- Change to desired format (JSON or XML)
- Next backup will use new format
- Previous logs remain in original format

### XML Parsing Errors

- XML files require proper structure
- Manual editing of XML logs not recommended
- If corrupted, delete problematic .xml file
- Logger will create new file on next backup

### Log Analysis

- **TransferTime > 0**: Successful transfer (time in milliseconds)
- **TransferTime = 0**: File not copied (differential backup, file unchanged)
- **TransferTime < 0**: Error occurred during transfer
- **ErrorMessage != null**: Details about the error

#### NEW in v3.0: Parallel Execution & Priority Issues

##### Jobs Not Running in Parallel

**Symptom**: Only one job running at a time

**Causes**:
1. Large file is transferring (blocks other large files)
2. Priority files still pending on another job
3. Parallel execution disabled in settings

**Solution**:
1. Check Settings → Parallel Execution (must be enabled)
2. Wait for large file to complete
3. Small files should transfer in parallel with large file
4. Check priority extensions configuration

##### Jobs Stuck on Priority Files

**Symptom**: Non-priority files waiting indefinitely

**Cause**: High-priority extensions still pending on another job

**Solution**:
1. This is expected behavior - priority files must complete first
2. Check which job has priority files remaining
3. Larger jobs may take longer for priorities to complete

##### Business Software Auto-Pause Not Working

**Symptom**: Jobs continue even though business software is detected

**Causes**:
1. Business software detection disabled in settings
2. Application name not exactly matching configured list
3. Process detection thread not executing

**Solution**:
1. Enable "Business Software Detection" in Settings
2. Verify exact executable names (case-sensitive):
   - Use Task Manager → Processes tab
   - Check exact name of running process
   - Add to config.json
3. Check DetectionInterval setting (default 5000 ms)
4. Restart EasySave if just enabled

##### CryptoSoft Mono-Instance Error

**Error**: "CryptoSoft is already running on this system"

**Cause**: CryptoSoft instance still running (locked by mutex)

**Solution**:
1. Check Task Manager for CryptoSoft.exe
2. Kill the process if hung: `taskkill /IM CryptoSoft.exe /F`
3. Delete lock file: `%AppData%\ProSoft\CryptoSoft\*.lock`
4. Retry CryptoSoft launch
5. If persistent, reboot machine

##### Centralized Logging Connection Failed

**Error**: "Cannot connect to Docker service at http://..."

**Causes**:
1. Docker service is not running
2. Network connectivity issue
3. Wrong service URL in configuration
4. Firewall blocking port 8080

**Solution**:
1. Verify Docker service is running:
   ```bash
   docker-compose ps
   ```
2. Check network connectivity:
   ```powershell
   Test-NetConnection log-service.internal -Port 8080
   ```
3. Verify URL in config.json matches running service
4. Check firewall rules allow port 8080
5. Local fallback is active - logs saved locally until service available

##### Logs Not Syncing to Docker

**Symptom**: Local logs created but not in Docker service

**Causes**:
1. Centralized logging disabled in settings
2. Docker service URL unreachable
3. Service authentication failed
4. Network latency

**Solution**:
1. Check "Centralized Logging Enabled" in Settings
2. Verify Docker service is healthy:
   ```bash
   curl http://log-service.internal:8080/api/health
   ```
3. Check logs for connection errors:
   - Local logs: `%AppData%\ProSoft\EasySave\Logs\`
4. Increase SyncInterval if under heavy backup load
5. Check event logs for network errors

##### Large File Threshold Not Limiting Transfer

**Symptom**: Multiple large files transferring simultaneously

**Cause**: Large file threshold set too high or disabled

**Solution**:
1. Check `LargeFileThresholdKB` in config.json
2. Verify threshold is lower than your actual large files
3. Default 5000 KB (5 MB) - adjust per your network
4. Restart EasySave for changes to take effect

---

## Technical Architecture

### Components

1. **EasySave.GUI.exe**: Graphical user interface (Avalonia/MVVM)
2. **EasySave.Console.exe**: Console application (CLI)
3. **EasyLog.dll**: Logging library (JSON/XML/Centralized)
4. **EasySave.Core.dll**: Business logic (encryption, detection, parallel backup)
5. **EasySave.Shared.dll**: Shared models and configuration

### NEW in v3.0: Architecture Enhancements

#### Parallel Execution Engine

- **Thread Pool**: System.Threading.TaskScheduler for job coordination
- **Concurrent Collections**: ConcurrentQueue for file scheduling
- **Lock-free synchronization**: Atomic operations where possible
- **Checkpoint System**: Save/restore state for pause/resume

#### Priority File Management

```
PriorityQueue Implementation:
├── High Priority (Extensions in PriorityExtensions list)
├── Normal Priority (remaining files)
└── Large File Constraint (exclusive transfers > n KB)
```

#### Business Software Monitor

- **Background Thread**: Dedicated thread for process monitoring
- **Interval-based Check**: Configurable detection frequency
- **Automatic Pause/Resume**: Seamless coordination with transfer threads
- **Low CPU Overhead**: <1% on typical systems

#### CryptoSoft Mono-Instance

- **Mutex Synchronization**: `System.Threading.Mutex`
- **Global Mutex Name**: `"CryptoSoft_Instance_Mutex"`
- **Per-Machine Isolation**: Machine-scoped, not network-scoped
- **Lock File Cleanup**: Automatic on graceful exit

#### Centralized Logging

- **HTTP REST API**: ASP.NET Core service
- **Async Operations**: Non-blocking log submissions
- **Offline Fallback**: Local retry queue if service unavailable
- **User/Computer Tracking**: Automatic identification

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

**Document Version**: 3.0
**Last Updated**: February 25, 2026
**ProSoft Technical Support Team**
