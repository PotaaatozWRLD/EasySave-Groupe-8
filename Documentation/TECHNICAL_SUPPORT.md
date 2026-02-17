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

**Generic Structure**:

```json
{
  "Language": "fr",
  "LogFormatString": "JSON",
  "BusinessSoftwareNames": [ "calc", "notepad" ],
  "ExtensionsToEncrypt": [ ".docx", ".pdf" ],
  "PriorityExtensions": [ ".txt", ".csv" ],
  "MaxLargeFileSizeKB": 1000,
  "EnableNetworkLogging": true,
  "LogServerIp": "127.0.0.1",
  "LogServerPort": 9000,
  "CryptoSoftPath": "Path\\To\\CryptoSoft.exe"
}
```

**New Fields in V3.0**:

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

For centralized logging, a Docker container is provided.

### Deployment

1.  Navigate to the installation directory containing `docker-compose.yml`.
2.  Run the server:
    ```powershell
    docker compose up -d
    ```
3.  Allow firewall access on port **9000** if running on a separate server.

### Troubleshooting Docker

-   **Connection Refused**: Ensure the container is running (`docker ps`) and port 9000 is mapped.
-   **No Logs**: Check `config.json` has `"EnableNetworkLogging": true`.

---

## Troubleshooting V3.0 Features

### 1. Backups are Paused Automatically

**Cause**: A business software (e.g., Calculator) is running.
**Solution**: Close the business software defined in Settings. The backup will resume automatically.

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
**Last Updated**: Feb 2026
