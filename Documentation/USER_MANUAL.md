# EasySave 3.0 - User Manual

## 1. Installation
1.  Ensure **.NET 8.0 SDK** or Runtime is installed.
2.  Extract the `EasySave` archive to your desired location.
3.  To use the CryptoSoft features, ensure `CryptoSoft.exe` is present in the `CryptoSoft/bin/Release/net10.0/` directory (or configure the path).
4.  (Optional) For Centralized Logging, ensure **Docker** is installed and running.

EasySave 3.0 is a professional backup software that allows you to create and manage unlimited backup jobs with parallel execution capabilities. Each job can be configured to perform a full or differential backup with optional AES-256 encryption. Version 3.0 adds parallel backup execution, priority file management, real-time job control, automatic pause on business software detection, and centralized Docker-based logging.

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

#### Via GUI

- **Job List**: See all backups with status badges (IDLE, RUNNING, SUCCESS, ERROR)
- **Edit Job**: Double-click any job or use Edit button
- **Delete Job**: Select job and click Delete (confirmation required)
- **Create Job**: Click Create New Job button
- **View Details**: Click on job to see configuration and encryption settings

#### Via Console (Interactive)

- **List Jobs** (Option 1): View all configured backups
- **Edit Job** (Option 3): Modify an existing backup configuration
- **Delete Job** (Option 4): Remove a backup job

### Running Backups

#### Via GUI

1. Select one or more jobs from the list
2. Click **Run Backup** or **Run All Jobs**
3. Watch real-time progress:
   - Overall progress bar (0-100%)
   - Current file being transferred
   - Transfer speed and estimated time
   - Pause/Resume/Stop controls
4. View detailed log after completion

**NEW in v3.0: Parallel Execution**
- All selected jobs run **simultaneously** instead of sequentially
- Backup time reduced by 40-60% depending on job count
- Progress tracked independently for each job
- Large files (>n KB) transfer exclusively to prevent bandwidth saturation
- Priority files complete before non-priority files

#### Via Console (Interactive)

- **Run Single Job** (Option 5): Execute one specific backup
- **Run All Jobs** (Option 6): Execute all configured backups in parallel
  - Examples: Enter "1" for job 1, or "1-3" for jobs 1 to 3, or "1;3" for jobs 1 and 3

### Job Control (NEW v3.0)

**Real-time pause, resume, and stop controls for backup jobs:**

#### Individual Job Control (via GUI)

For each running backup job:

1. **⏸️ Pause Button**
   - Pauses the job after current file completes
   - Safe checkpoint - no data loss
   - Other jobs continue running

2. **▶️ Play/Resume Button**
   - Resumes a paused job
   - Continues from checkpoint
   - Differential backup continues where it left off

3. **⏹️ Stop Button**
   - Immediately terminates the job
   - Saves progress to state file
   - Can be resumed later (starts from checkpoint)

#### Global Controls (via GUI)

Operate on **all running jobs simultaneously**:

- **Pause All**: Pauses every job after current file
- **Resume All**: Resumes all paused jobs
- **Stop All**: Terminates all jobs immediately

#### Status Indicators

Each job shows its current state:
- 🟢 **RUNNING**: Job actively transferring files
- 🟡 **PAUSED**: Job waiting for resume
- ⏹️ **STOPPED**: Job halted, waiting for action
- ✅ **COMPLETE**: Job finished successfully
- ❌ **ERROR**: Job encountered error

### Priority File Management (NEW v3.0)

**Intelligent scheduling ensures critical files backup first:**

#### How Priority Works

1. **Administrator configures priority extensions** in Settings
   - Example: `.docx`, `.xlsx`, `.pptx`, `.sql`
   - These files are considered "critical"

2. **During parallel execution:**
   - Priority files are transferred FIRST across all jobs
   - Non-priority files wait until NO priority files remain
   - Prevents critical business documents from taking too long to backup

#### Example Scenario

```
Job A has: [INVOICE.docx] [photo.jpg] [backup.sql]
Job B has: [REPORT.xlsx] [video.mp4] [notes.txt]

Priority extensions: .docx, .xlsx, .sql
Large file threshold: 5 MB

Transfer order:
1. Job A: INVOICE.docx (HIGH PRIORITY)
2. Job B: REPORT.xlsx (HIGH PRIORITY)
3. Job A: backup.sql (HIGH PRIORITY)
4. Job B: notes.txt (non-priority, OK to start)
5. Job A: photo.jpg (non-priority)
6. Job B: video.mp4 (>5MB, exclusive transfer)
```

#### Configuring Priority Extensions

1. Click **Settings** in main menu
2. Scroll to **Priority File Management**
3. Enter file extensions (comma-separated):
   ```
   .docx,.xlsx,.pptx,.accdb,.sql
   ```
4. Click **Save**
5. Next backup uses new priority list

### Settings

#### Via GUI (Recommended)

1. Click **Settings** in the main menu
2. Configure options:
   - **Language**: English or French
   - **Log Format**: JSON or XML
   - **Business Software Detection**: Enable/disable automatic prevention
   - **Business Software List**: Add or remove applications to monitor
   - **Encryption Settings**: Default compression and algorithm
3. Changes apply immediately

#### Via Console (Interactive)

1. Select **Option 9** (Settings) from main menu
2. View current settings
3. Change desired options
4. Confirm changes (applied immediately to next backup)

### Business Software Detection (NEW v3.0)

**Automatic pause when business applications are detected:**

#### How It Works

1. **Background monitoring**: System continuously monitors running applications
2. **If business software detected**:
   - ⏸️ ALL active backup jobs **automatically pause**
   - Clear notification displayed: "Business software detected - Backup paused"
   - No user action required
3. **When software closes**:
   - ✅ Jobs **automatically resume** from checkpoint
   - Backup continues seamlessly
   - Full transparency to user

#### Default Monitored Applications

- Microsoft Office: Word (WINWORD.EXE), Excel (EXCEL.EXE), PowerPoint (POWERPNT.EXE)
- Databases: SQL Server (sqlservr.exe), MySQL (MYSQLD.EXE)
- Custom applications (configurable)

#### User Experience Example

```
14:30:15 → Backup running normally
14:30:45 → User opens Excel (EXCEL.EXE)
14:30:46 ⏸️ PAUSED: Business software detected (Excel)
           All backup jobs paused after current file.
           
14:32:10 → User closes Excel
14:32:11 ▶️ RESUMED: All jobs resumed automatically

Checkpoint restored - backup continues without interruption
```

#### Customizing Monitored Applications

1. Open **Settings** in GUI
2. Go to **Business Software Detection**
3. Add/remove application executable names:
   ```
   WINWORD.EXE
   EXCEL.EXE
   POWERPNT.EXE
   OUTLOOK.EXE
   sqlservr.exe
   MYSQLD.EXE
   [Add custom apps]
   ```
4. Click **Save** - takes effect immediately
5. Disable entirely if needed (checkbox)

### Command Line Usage (Console Version)

Run backups automatically without the interactive menu:

**Syntax**:

```bash
EasySave.Console.exe <job_specification>
```

**Examples**:

```bash
EasySave.Console.exe 1         # Execute job 1
EasySave.Console.exe 1-3       # Execute jobs 1, 2, and 3
EasySave.Console.exe 1;3;5;10  # Execute specific jobs
EasySave.Console.exe 2-4       # Execute range of jobs
```

**Perfect for**:
- Scheduled tasks (Windows Task Scheduler)
- Automated scripts
- CI/CD pipelines
- Batch operations

**Note**: Encryption and business software detection work in CLI mode too

## Large File Management (NEW v3.0)

**Prevent bandwidth saturation with large file constraints:**

### Large File Threshold Configuration

To protect network bandwidth, only ONE file larger than the threshold can transfer simultaneously. Small files (<threshold) from other jobs can continue.

#### Setting the Threshold

1. Open **Settings** in GUI
2. Go to **Performance Settings**
3. Find **Large File Threshold (KB)**
4. Enter size in kilobytes:
   ```
   5000  (5 MB - recommended for most networks)
   1000  (1 MB - for slower networks)
   10000 (10 MB - for fast networks)
   ```
5. Click **Save**

#### Example with Large File Constraint

```
Large file threshold: 5000 KB

Job A: file1.xlsx (6 MB) ← Large file, blocks other large transfers
Job B: backup.docx (800 KB) → Can transfer (small)
Job C: video.mp4 (50 MB) ← Waits (large, and Job A has large file)
Job D: photo.jpg (2 MB) → Can transfer (small)

Result:
- Job A: Large file transfers exclusively
- Job B: Small file transfers in parallel
- Job D: Small file transfers in parallel
- Job C: Waits until Job A completes
```

### Bandwidth Impact

- **Without constraint**: Multiple large files compete for bandwidth → slower overall
- **With constraint**: One large file uses full bandwidth → faster overall
- **Recommendation**: Set to 40-50% of your typical available bandwidth

---

## Centralized Logging (NEW v3.0)

**Optional Docker-based log aggregation for enterprise deployments:**

### Three Deployment Modes

#### 1. Local Only (Default)

- Logs stored in: `%AppData%\ProSoft\EasySave\Logs\`
- Only this PC's logs retained
- Perfect for: Single users, small offices
- **Configuration**: Done by default

#### 2. Centralized Only

- Logs sent to Docker service
- No logs stored locally
- Perfect for: Enterprise deployments, compliance auditing
- Requires: Docker service running

#### 3. Hybrid (Local + Centralized)

- Logs stored locally AND sent to Docker
- Redundancy and offline access
- Perfect for: High-availability environments
- Requires: Docker service (with local fallback)

### Enabling Centralized Logging

#### Prerequisites

1. **Docker Service Running**
   - System administrator deploys Docker service
   - Service URL provided (e.g., `http://logs.company.internal:8080`)
   - Service must be accessible from your PC

#### Configuration Steps

1. Open **Settings** in EasySave GUI
2. Scroll to **Logging Configuration**
3. Check **Enable Centralized Logging**
4. Enter Docker service URL:
   ```
   http://logs.company.internal:8080
   ```
5. Choose mode:
   - ☐ Local only
   - ☑ Centralized only
   - ☑ Hybrid (local + centralized)
6. Click **Save**

### Benefits of Centralization

- **Single source of truth**: All user backups in one place
- **Audit trail**: Who backed up what, when
- **Compliance**: Meet regulatory requirements
- **Analytics**: View trends across entire company
- **Faster investigation**: When issues occur

### Troubleshooting Centralized Logging

- **"Cannot reach Docker service"**: Check URL and network connectivity
- **Local fallback active**: Service unavailable, logs stored locally
- **Logs appear delayed**: Normal, sync interval 5-10 seconds
- **Contact IT**: If Docker service is down

## Encryption (NEW v2.0)

### Selective Encryption

- **Per-job encryption**: Configure which jobs encrypt files
- **By extension**: Encrypt only specific file types (.docx, .xlsx, .pdf, etc.)
- **AES-256**: Military-grade encryption for sensitive data
- **Transparent**: Works alongside backup without extra configuration

### Setting Up Encryption

1. Open job in GUI or create new job
2. Enable **Encryption**
3. Enter file extensions to encrypt (comma-separated)
4. Save job
5. Next backup will automatically encrypt matching files

### Encryption Log Details

- **EncryptionTime**: Time taken to encrypt file (in milliseconds)
- **EncryptionTime = 0**: File not encrypted (extension not selected)
- **EncryptionTime > 0**: File successfully encrypted
- **EncryptionTime < 0**: Encryption failed (file backed up unencrypted)

### CryptoSoft Mono-Instance (NEW v3.0)

EasySave integrates with CryptoSoft encryption service. CryptoSoft runs as a **single instance per machine** for security:

- **Can't launch CryptoSoft twice**: Only one instance allowed on your PC
- **If already running**: Error message shown, second launch blocked
- **After closing**: Next launch works normally

**User Impact**: Minimal - automatic background process, user doesn't interact directly

## Log Files

EasySave creates log files in: `%AppData%\ProSoft\EasySave\Logs\`

- Daily logs: Contains details of all file transfers (format: YYYY-MM-DD.json or YYYY-MM-DD.xml)
- State file: Shows real-time progress of active backups (state.json or state.xml)
- **Log Format** (v1.1): Choose between JSON (default) or XML format via Settings menu

## Supported Locations

EasySave can backup from/to:

- Local drives (C:\, D:\, etc.)
- External drives (USB, external hard drives)
- Network drives (\\server\share)

## Troubleshooting

- **"Cannot add more than 5 jobs"**: This limitation was removed in v2.0. You can create unlimited jobs.
- **Access denied errors**: Ensure you have read/write permissions on source and target folders.
- **Job not found**: Verify the job number in the list (Option 1).

### NEW in v3.0

- **Job paused unexpectedly**: Business software detected. Close the application and it will resume automatically.
- **Jobs not running in parallel**: Check if large file is transferring (only one >n KB at a time). Wait for completion.
- **Jobs stuck on priority file**: Other jobs waiting for priority files to complete. This is expected behavior.
- **"CryptoSoft already running"**: Close the existing CryptoSoft instance. Only one can run per machine.
- **Centralized logs not syncing**: Check Docker service URL in Settings. Local fallback active if unavailable.

## Support

## 4. Centralized Logging (Docker)
To enable the log server:
1.  Open a terminal in the project root.
2.  Run `docker-compose up -d`.
3.  Set `"EnableNetworkLogging": true` in `config.json`.
4.  Logs will now be visible in the Docker container (`docker logs easysave-log-server`).

---
**ProSoft - EasySave Version 3.0**
© 2026 ProSoft. All rights reserved.
