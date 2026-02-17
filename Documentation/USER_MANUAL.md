# EasySave V3.0 - User Manual

## 1. Installation
1.  Ensure **.NET 8.0 SDK** or Runtime is installed.
2.  Extract the `EasySave` archive to your desired location.
3.  To use the CryptoSoft features, ensure `CryptoSoft.exe` is present in the `CryptoSoft/bin/Release/net10.0/` directory (or configure the path).
4.  (Optional) For Centralized Logging, ensure **Docker** is installed and running.

## 2. Configuration (`config.json`)
The application configuration is stored in `%APPDATA%\ProSoft\EasySave\config.json`. You can modify it manually or let the application create default values.

### Key Settings:
- **`Language`**: `en` (English) or `fr` (French).
- **`LogFormatString`**: `JSON` or `XML`.
- **`ExtensionsToEncrypt`**: List of extensions to encrypt (e.g., `[".docx", ".pdf"]`).
- **`BusinessSoftwareNames`**: List of process names that, if running, will **Pause** backups (e.g., `["notepad", "calc"]`).
- **`PriorityExtensions`**: Files to back up first (e.g., `[".txt", ".csv"]`).
- **`MaxLargeFileSizeKB`**: Files larger than this (in KB) will typically wait for a "slot" to prevent lagging the PC (Default: `1024`).
- **`EnableNetworkLogging`**: Set to `true` to send logs to a server.
- **`LogServerIp`**: IP address of the log server (Default: `127.0.0.1`).

## 3. Usage (GUI)

### Managing Jobs
- **Create:** Click the "+" button to add a new backup job.
- **Edit:** Select a job and click the "Pen" icon.
- **Delete:** Select a job and click the "Trash" icon.

### Executing Backups
- **Run Selected:** Select one or more jobs and click the "Play" button (top right).
- **Run All:** Click the "Play All" button (double arrow) to start everything.

### Controlling Execution (New in V3.0!)
- **Pause/Resume:** Click the **Pause (⏸)** button next to a running job (or the global "Pause All" at the bottom) to temporarily suspend activity. Click **Play (▶)** to resume.
- **Stop:** Click the **Stop (⏹)** button to cancel a job immediately.

### Business Software Detection
- If you launch a configured business software (e.g., Notepad) while backups are running, EasySave will **automatically pause** all jobs.
- Once you close the software, EasySave will **automatically resume**.

## 4. Centralized Logging (Docker)
To enable the log server:
1.  Open a terminal in the project root.
2.  Run `docker-compose up -d`.
3.  Set `"EnableNetworkLogging": true` in `config.json`.
4.  Logs will now be visible in the Docker container (`docker logs easysave-log-server`).

---
*For support, contact the ProSoft IT Department.*
