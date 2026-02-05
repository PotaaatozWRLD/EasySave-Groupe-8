# EasySave 1.0 - User Manual

## Introduction
EasySave is a backup software that allows you to create and manage up to 5 backup jobs. Each job can be configured to perform a full or differential backup.

## Getting Started

### Launching EasySave

**Interactive Mode** (with menu):
1. Double-click `EasySave.Console.exe` or run from command prompt:
   ```
   EasySave.Console.exe
   ```
2. Select your language (English or French)
3. Use the interactive menu to manage backups

**Command Line Mode** (automated execution):
```
EasySave.Console.exe 1        (Run job #1)
EasySave.Console.exe 1-3      (Run jobs 1 through 3)
EasySave.Console.exe 1;3;5    (Run jobs 1, 3, and 5)
```

### Creating a Backup Job
1. Launch EasySave.Console.exe in interactive mode
2. Select language (English or French)
3. Choose option **2** from the menu to create a new job
4. Enter:
   - **Name**: A unique name for the backup
   - **Source Path**: The directory to backup (e.g., C:\Documents)
   - **Target Path**: Where to save the backup (e.g., D:\Backups)
   - **Type**: "Full" for complete backup or "Differential" for changes only

### Managing Jobs
- **List Jobs** (Option 1): View all configured backups
- **Edit Job** (Option 3): Modify an existing backup configuration
- **Delete Job** (Option 4): Remove a backup job

### Running Backups
- **Run Single Job** (Option 5): Execute one specific backup
  - Examples: Enter "1" for job 1, or "1-3" for jobs 1 to 3, or "1;3" for jobs 1 and 3
- **Run All Jobs** (Option 6): Execute all configured backups sequentially

### Command Line Usage
Run backups automatically without the interactive menu:

**Syntax**:
```
EasySave.Console.exe <job_specification>
```

**Examples**:
```
EasySave.Console.exe 1         → Execute job 1
EasySave.Console.exe 1-3       → Execute jobs 1, 2, and 3 sequentially
EasySave.Console.exe 1;3       → Execute jobs 1 and 3
EasySave.Console.exe 2-4       → Execute jobs 2, 3, and 4
```

**Location**: The executable is located in:
```
EasySave\EasySave.Console\bin\Release\net10.0\win-x64\publish\EasySave.Console.exe
```

## Backup Types
- **Full Backup**: Copies all files from source to target
- **Differential Backup**: Only copies files that have been modified since the last backup

## Log Files
EasySave creates log files in: `%AppData%\ProSoft\EasySave\Logs\`
- Daily logs: Contains details of all file transfers
- State file: Shows real-time progress of active backups

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
For technical assistance, contact ProSoft support at support@prosoft.com

---
**ProSoft - EasySave Version 1.0**
© 2026 ProSoft. All rights reserved.
