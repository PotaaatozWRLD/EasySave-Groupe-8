# EasySave 2.0 - User Manual

## Introduction

EasySave is a professional backup software that allows you to create and manage **unlimited** backup jobs with modern graphical interface, automatic file encryption, and business software detection. This version features both a user-friendly GUI (Avalonia) and command-line interface for automation.

## Getting Started

### Launching EasySave

**Graphical User Interface (Recommended):**

1. Double-click `EasySave.GUI.exe`
2. Main window opens with all your backup jobs listed
3. Create, edit, delete, or execute jobs using the intuitive UI
4. Monitor real-time progress during backup execution

**Console/Interactive Mode:**

1. Run from command prompt:
   ```
   EasySave.Console.exe
   ```
2. Select your language (English or French)
3. Use the menu to manage backups (compatible with v1.0/1.1)

**Command Line Mode** (automated execution - no prompts):

```bash
EasySave.Console.exe 1        # Run job #1 directly
EasySave.Console.exe 1-3      # Run jobs 1 through 3 automatically
EasySave.Console.exe 1;3;5;10 # Run specific jobs automatically
```

*Note: In command line mode, backups execute immediately without any prompts.*

### Creating a Backup Job

#### Via GUI (Recommended)

1. Launch `EasySave.GUI.exe`
2. Click **Create New Job** button
3. Fill in the Job Editor form:
   - **Name**: Unique name for the backup
   - **Source Path**: Directory to backup (e.g., C:\Documents)
   - **Target Path**: Backup destination (e.g., D:\Backups)
   - **Type**: Full or Differential backup
   - **Encryption** (NEW v2.0):
     - Enable/disable encryption
     - Select file extensions to encrypt (e.g., .docx, .xlsx)
   - **Validation**: Form validates paths and prevents duplicate names
4. Click **Save Job**

#### Via Console (Interactive)

1. Run `EasySave.Console.exe`
2. Select language (English or French)
3. Choose option **2** to create a new job
4. Answer prompts for Name, Source, Target, Type

**Note**: GUI provides better validation and encryption configuration

### Managing Jobs

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

#### Via Console (Interactive)

- **Run Single Job** (Option 5): Execute one specific backup
- **Run All Jobs** (Option 6): Execute all configured backups sequentially
  - Examples: Enter "1" for job 1, or "1-3" for jobs 1 to 3, or "1;3" for jobs 1 and 3

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

### Business Software Detection (NEW v2.0)

**Protects your data from corruption when business applications are using files:**

1. Application checks if configured software is running
2. If detected, backup is **blocked** with clear message
3. Suggestion: Close the application and retry
4. Once software exits, backup can execute normally

**Default monitored applications:**
- Microsoft Office (Word, Excel, PowerPoint)
- Databases (SQL Server, MySQL)
- Custom applications (configurable)

**Manage in Settings:**
- View current monitored list
- Add custom applications
- Disable detection if not needed

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

## Backup Types

- **Full Backup**: Copies all files from source to target (slower, comprehensive)
- **Differential Backup**: Only copies files modified since the last backup (faster, efficient)

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

### Performance Impact

- Encryption adds ~10-20% overhead depending on file size
- Large files take longer to encrypt
- Recommended: encrypt outside peak hours for network drives

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

- **"Cannot add more than 5 jobs"**: Maximum limit reached. Delete an existing job first.
- **Access denied errors**: Ensure you have read/write permissions on source and target folders.
- **Job not found**: Verify the job number in the list (Option 1).

## Support

For technical assistance, contact ProSoft support at <support@prosoft.com>

---
**ProSoft - EasySave Version 1.1**
© 2026 ProSoft. All rights reserved.
