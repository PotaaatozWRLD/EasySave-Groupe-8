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
- **Framework**: .NET 8.0 Runtime
- **Processor**: 1 GHz or faster
- **RAM**: 512 MB minimum, 1 GB recommended
- **Disk Space**: 50 MB for application, additional space for logs
- **Permissions**: Read access to source directories, Write access to target directories
- **Docker**: Docker Desktop (Required for Centralized Logging feature)

### Network Requirements

- Network access if using UNC paths (\\server\share)
- SMB/CIFS protocol support for network drives
- **TCP Port 9000** (Default): Outbound access to Docker Log Server (if usage enabled)

---

## Installation

### Default Installation Locations

- **Executable**: `EasySave.GUI.exe` or `EasySave.Console.exe`
- **Configuration Files**: `%AppData%\ProSoft\EasySave\`
- **Log Files**: `%AppData%\ProSoft\EasySave\Logs\`

### Running the Application

**Method 1: Graphical User Interface (GUI)**

```powershell
EasySave.GUI.exe
```

Launches the Avalonia-based GUI with V3.0 features:
1.  **Parallel Execution Dashboard**: Real-time progress of multiple jobs.
2.  **Job Control**: Play/Pause/Stop individual or all jobs.
3.  **Settings**: Configuration of Priority Files, Throttling, and Docker Logs.

**Method 2: Console Interactive Mode**

```powershell
EasySave.Console.exe
```

**Method 3: Command Line Mode**

```powershell
EasySave.Console.exe 1-3
```

---

## Configuration Files

### config.json

**Location**: `%AppData%\ProSoft\EasySave\config.json`
**Purpose**: General application settings.

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

-   **`PriorityExtensions`**: List of file extensions (e.g., `[".txt"]`) that will be backed up *before* any other files.
-   **`MaxLargeFileSizeKB`**: Files larger than this size (in KB) will NOT be transferred in parallel with other large files to prevent disk saturation.
-   **`EnableNetworkLogging`**: `true` to send logs to the Docker server.
-   **`LogServerIp`**: IP address of the Docker container (default `127.0.0.1`).
-   **`LogServerPort`**: Port of the listening server (default `9000`).

### jobs.json

**Location**: `%AppData%\ProSoft\EasySave\jobs.json`
**Purpose**: Stores backup job definitions.

---

## Docker Log Server (New in V3.0)

### Encryption Failed (NEW v2.0)

### Deployment

1.  Navigate to the installation directory containing `docker-compose.yml`.
2.  Run the server:
    ```powershell
    docker compose up -d
    ```
3.  Allow firewall access on port **9000** if running on a separate server.

### Troubleshooting Docker

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

## Troubleshooting V3.0 Features

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

### 2. "Throttling" / Slow Backup of Large Files

**Cause**: Files larger than `MaxLargeFileSizeKB` are processed sequentially to protect disk I/O.
**Solution**: Increase the limit in Settings if your hardware supports high I/O (e.g., SSD NVMe).

### 3. Priority Files Blocking

**Cause**: Non-priority files wait until ALL priority files (e.g., `.txt`) from ALL active jobs are finished.
**Solution**: This is intended behavior. To disable, remove extensions from the Priority list.

### 4. CryptoSoft Error

**Cause**: CryptoSoft is now Mono-Instance.
**Solution**: EasySave V3.0 automatically handles queuing. If manually running CryptoSoft, ensure only one instance is active.

---

## Technical Support Escalation

**Support Email**: support@prosoft.com
**Hotline**: 8:00 - 17:00 (Mon-Fri)

**Required Info for Tickets**:
1.  `config.json` content.
2.  Logs from `%AppData%\ProSoft\EasySave\Logs\`.
3.  Docker container status (if network logging used).

**Document Version**: 3.0
**Last Updated**: February 25, 2026
**ProSoft Technical Support Team**
