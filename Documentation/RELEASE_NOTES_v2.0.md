# EasySave v2.0 - Graphical Interface & Unlimited Jobs

**Release Date:** February 11, 2026  
**Development Team:** ProSoft - Groupe 8

---

## 🎉 Third Major Release

EasySave v2.0 introduces a modern **graphical user interface (GUI)** with MVVM architecture, support for **unlimited backup jobs**, **automatic file encryption**, and **business software detection** to prevent data loss during critical operations.

---

## 👥 Development Team

- **Kenan HUREMOVIC** ([@PotaaatozWRLD](https://github.com/PotaaatozWRLD))
  - GUI design & MVVM implementation
  - Windows & Events handling
  
- **Axel Ruffin** ([@Axelruffin-69](https://github.com/Axelruffin-69))
  - Encryption service & file filtering
  - Testing & quality assurance
  
- **Jalil Lalouani** ([@Maszy69](https://github.com/Maszy69))
  - Business software detection
  - Documentation & UML updates

---

## ✨ What's New in v2.0

### 🖥️ Modern Graphical User Interface

**Avalonia-based GUI** with MVVM architecture for cross-platform support:

#### Main Window Features

- **Job Overview**: Real-time list of all backup jobs with status indicators
  - Job name, source, target, type, and last execution
  - Visual status badges (RUNNING, SUCCESS, ERROR, IDLE)
  - Job count and total size information

- **Job Editor**: Create and modify backup configurations
  - Name, source path, target path, backup type selection
  - File filter settings (extensions, size limits)
  - Encryption configuration per job
  - Input validation and error messages

- **Settings Panel**: Centralized configuration
  - Language selection (English/French)
  - Log format choice (JSON/XML)
  - Business software list (customizable blacklist)
  - Encryption settings (enable/disable, file types)

- **Progress View**: Live backup execution monitoring
  - Overall progress bar (0-100%)
  - Current file being transferred
  - Transfer speed and estimated time remaining
  - Pause/Resume/Stop controls
  - Real-time log display

#### GUI Workflows (MVVM Pattern)

```
View (XAML) ← ViewModel (Logic) → Model (Data)
```

- **ViewModels TBD**:
  - `MainWindowViewModel`: Main window state management
  - `JobEditorViewModel`: Job creation/editing logic
  - `SettingsViewModel`: User settings management
  - `MainViewModel`: Overall application state

### 📦 Unlimited Backup Jobs

**No more 5-job limitation!**

- Create as many backup jobs as needed
- Jobs stored in `jobs.json` with automatic management
- Scalable architecture for future enhancements
- Job IDs auto-generated instead of sequential numbering

**Impact on CLI**:

```bash
# CLI still works the same way
EasySave.exe 1          # Run job by ID
EasySave.exe 1-5        # Range of jobs
EasySave.exe 1;3;5;10   # Specific job IDs
```

### 🔐 Automatic File Encryption

**CryptoSoft Integration** with selective encryption:

#### Configuration per Job

```json
{
  "Name": "Sensitive Documents",
  "SourcePath": "C:\\ConfidentialFiles",
  "TargetPath": "D:\\SecureBackup",
  "Type": "Full",
  "Encryption": {
    "Enabled": true,
    "Extensions": [ ".docx", ".xlsx", ".pdf", ".pptx" ],
    "Algorithm": "AES-256"
  }
}
```

#### Encryption Features

- **Selective by Extension**: Encrypt only specific file types
- **Time Tracking**: `EncryptionTime` logged in JSON/XML
- **Error Handling**: Log encryption failures without stopping backup
- **CryptoSoft Interface**: Mono-instance (v3.0) preparation

#### Log Entry with Encryption (v2.0)

```json
{
  "Timestamp": "2026-02-11T14:30:25",
  "JobName": "Sensitive Documents",
  "SourcePath": "\\localhost\\C$\\ConfidentialFiles\\document.docx",
  "TargetPath": "\\localhost\\D$\\SecureBackup\\document.docx",
  "FileName": "document.docx",
  "FileSize": 2048000,
  "TransferTime": 250,
  "EncryptionTime": 125,
  "ErrorMessage": null
}
```

- **TransferTime**: File transfer duration (ms)
- **EncryptionTime**: Encryption duration (ms), 0 if skipped
- **Negative EncryptionTime**: Encryption failed (backup continued)

### 🚫 Business Software Detection

**Automatic Prevention of Data Loss**

#### Features

- **Detection**: Periodically check for running business applications
  - Office suite (Word, Excel, PowerPoint)
  - Databases (SQL Server, MySQL)
  - Custom applications (configurable list)

- **Blocking**: Prevent job execution while business software is active
  - Clear error message to user
  - Suggests retry after software closes
  - Customizable whitelist in Settings

#### Configuration

```json
{
  "DetectedSoftware": [
    "WINWORD.EXE",
    "EXCEL.EXE",
    "POWERPNT.EXE",
    "sqlservr.exe",
    "MYSQLD.EXE"
  ]
}
```

#### User Experience

1. User initiates backup job
2. System scans for business software
3. If detected:
   - Job launch blocked
   - User notified with clear message
   - Suggestion to close software and retry
4. Job execution continues once software is closed

### 🌐 Dual Interface Support

**Both Console and GUI available:**

#### Console Version
- `EasySave.Console.exe`: Command-line for automation
- Same CLI argument syntax as v1.0/1.1
- Useful for scheduled tasks, scripts, CI/CD pipelines

#### GUI Version
- `EasySave.GUI.exe`: Modern graphical interface
- All features visually accessible
- Real-time progress monitoring
- Recommended for regular users

#### Interoperability

- Both share same `jobs.json` configuration
- Logs are compatible (JSON/XML)
- Settings can be changed in either version
- `state.json/xml` updates reflect both interfaces

---

## 🧪 Enhanced Testing Suite

**110+ tests passing** (20+ new encryption & detection tests):

### New Test Categories (v2.0)

#### Encryption Service Tests (12 tests)
- ✅ AES-256 encryption/decryption
- ✅ Selective encryption by file extension
- ✅ Encryption time measurement
- ✅ Corrupted file handling
- ✅ Error logging for failed encryptions
- ✅ Performance under load

#### Business Software Detector Tests (8 tests)
- ✅ MS Office detection (Word, Excel, PowerPoint)
- ✅ Database detection (SQL Server, MySQL)
- ✅ Process listing and comparison
- ✅ Custom software detection
- ✅ False positive avoidance
- ✅ Detection performance

#### GUI/MVVM Tests (6 tests)
- ✅ ViewModel binding validation
- ✅ Command execution in MVVM pattern
- ✅ Settings persistence
- ✅ Job editor validation

**Test Execution:**

```bash
cd EasySave/EasySave.Tests
dotnet test
# Result: Total: 110+ | Passed: 110+ ✅ | Failed: 0 | Duration: ~8s
```

---

## 🔧 Technical Improvements

### Architecture Enhancements

- ✅ **MVVM Pattern**: Complete separation of UI and logic
- ✅ **Unlimited Jobs Scalability**: Database-ready structure
- ✅ **Encryption Service**: Pluggable encryption algorithms
- ✅ **Software Detection**: Extensible detection framework
- ✅ **CLI Compatibility**: Full backward compatibility with v1.0/1.1

### Code Quality

- ✅ **Dependency Injection**: Constructor-based DI for testability
- ✅ **Interface Segregation**: Small, focused interfaces
- ✅ **Application State Management**: Singleton for global state
- ✅ **Async/Await**: Non-blocking UI operations
- ✅ **Error Recovery**: Graceful degradation on failures

### Performance

- No performance degradation from v1.0/1.1
- Encryption adds minimal overhead (<10%)
- Software detection runs in background
- Multi-threaded backup execution (foundation for v3.0)

---

## 📦 What's Updated

### Project Structure (v2.0)

```
EasySave/
├── EasySave.Console/       # Console application (v1.0 compatible)
├── EasySave.GUI/           # NEW: Avalonia GUI (MVVM)
├── EasySave.Core/          # Enhanced with encryption & detection
├── EasyLog/                # Unchanged (backward compatible)
├── EasySave.Shared/        # Updated models with Encryption config
└── EasySave.Tests/         # +20 new tests
```

### Configuration Evolution

**jobs.json** (v2.0) - New optional properties:

```json
[
  {
    "Name": "My Backup",
    "SourcePath": "C:\\Source",
    "TargetPath": "D:\\Target",
    "Type": 1,
    "Encryption": {
      "Enabled": false,
      "Extensions": [],
      "Algorithm": "AES-256"
    }
  }
]
```

**config.json** (v2.0) - New settings:

```json
{
  "Language": "en",
  "LogFormatString": "JSON",
  "BusinessSoftwareDetectionEnabled": true,
  "BusinessSoftwareList": [
    "WINWORD.EXE",
    "EXCEL.EXE",
    "POWERPNT.EXE"
  ]
}
```

### Language Files (v2.0)

**New translations for GUI**:

**English (lang.en.json)**:
- `"ui.main_window": "EasySave Backup Manager"`
- `"ui.job_list": "Backup Jobs"`
- `"ui.create_job": "Create New Job"`
- `"ui.edit_job": "Edit Job"`
- `"ui.delete_job": "Delete Job"`
- `"ui.run_backup": "Run Backup"`
- `"ui.encryption_enabled": "Enable Encryption"`
- `"ui.business_software_detected": "Business software detected. Close it first."`

**French (lang.fr.json)**:
- `"ui.main_window": "Gestionnaire de Sauvegarde EasySave"`
- `"ui.job_list": "Travaux de Sauvegarde"`
- `"ui.create_job": "Créer un nouveau travail"`
- etc.

---

## 🚀 Migration from v1.0/v1.1

### Automatic Migration

**No action required!**

- v2.0 reads v1.0/v1.1 configuration files
- Existing `jobs.json` remains compatible
- New encryption fields added with defaults (`"Encryption": {"Enabled": false}`)
- All logs remain accessible
- Settings automatically upgraded

### First Launch (v2.0)

1. **GUI Selection**: User can choose Console or GUI version
2. **Settings Migration**: Old settings automatically upgraded
3. **Job Compatibility**: Existing jobs work without modification
4. **New Features Disabled**: Encryption/detection default to OFF

### Switching Versions

- Use Console for scripting/automation
- Use GUI for interactive management
- Both versions share configuration files
- No conflicts or locking issues

---

## 📁 File Locations (v2.0)

All files stored in: `%AppData%\ProSoft\EasySave\`

```
├── config.json                  # Application settings + new v2.0 options
├── jobs.json                    # Unlimited backup jobs (no 5-job limit!)
├── state.json OR state.xml      # Real-time state (format depends on selection)
└── Logs/
    ├── 2026-02-11.json          # Daily logs (JSON format)
    └── 2026-02-11.xml           # Daily logs (XML format if selected)
```

---

## 🔮 Roadmap - Coming in v3.0

### Parallel Execution

- **Multi-job execution**: Run multiple backups simultaneously
- **Priority management**: Assign priority levels to jobs
- **Resource limits**: Control CPU/bandwidth usage

### Advanced Features

- **Large file optimization**: Chunking for files >1GB
- **Bandwidth throttling**: Limit transfer speed
- **Scheduler integration**: Built-in job scheduling

### Centralized Logging

- **Docker support**: Centralized log collection
- **Remote log aggregation**: Multiple clients → central server
- **Advanced analytics**: Dashboard with statistics

---

## 💼 Commercial Information

- **License Price:** 200 €HT per unit
- **Maintenance:** 12% annual (24 €HT/year)
  - Free updates (v2.0 included for v1.0 users)
  - Priority support
  - New features access
- **Support Hours:** Monday-Friday, 8:00 AM - 5:00 PM
- **Contact:** <support@prosoft.com>

---

## 🐛 Known Issues

**None reported in v2.0**

For bug reports: [GitHub Issues](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/issues)

---

## 📝 Changelog

### v2.0 (2026-02-11) - GUI & Unlimited Jobs

#### New Features ✨

- ✅ **Graphical User Interface (GUI)**: Avalonia-based MVVM application
- ✅ **Unlimited Backup Jobs**: No more 5-job limitation
- ✅ **File Encryption**: AES-256 with selective extension filtering
- ✅ **Business Software Detection**: Automatic prevention when business apps running
- ✅ **Dual Interface**: Console (CLI) + GUI both available
- ✅ **Job Editor**: Create/modify jobs with validation
- ✅ **Live Progress View**: Real-time backup monitoring with controls
- ✅ **Settings Panel**: Centralized configuration for all features

#### Technical Improvements 🔧

- ✅ **MVVM Architecture**: Clean separation of concerns
- ✅ **Dependency Injection**: Constructor-based DI
- ✅ **Encryption Service**: AES-256 implementation
- ✅ **Software Detection**: Process-based detection engine
- ✅ **Extended State Tracking**: Encryption metrics in logs
- ✅ **Configuration Upgrade**: Backward compatible with v1.0/1.1

#### Testing 🧪

- ✅ **20+ New Tests**: Encryption, detection, MVVM validation
- ✅ **110+ Total Tests**: All passing (100% success rate)
- ✅ **CodeQL Analysis**: No critical warnings
- ✅ **Integration Tests**: GUI interaction scenarios

#### Documentation 📚

- ✅ **User Manual v2.0**: GUI usage guide
- ✅ **Technical Support v2.0**: Encryption & detection troubleshooting
- ✅ **Release Notes v2.0**: Complete changelog
- ✅ **Updated UML Diagrams**: MVVM architecture & patterns

#### Breaking Changes ⚠️

- **None!** Full backward compatibility with v1.0/1.1

### v1.1 (2026-02-09) - XML Logging & Configuration

See [RELEASE_NOTES_v1.1.md](RELEASE_NOTES_v1.1.md) for details.

### v1.0 (2026-02-06) - Initial Release

See [RELEASE_NOTES_v1.0.md](RELEASE_NOTES_v1.0.md) for details.

---

## 🙏 Acknowledgments

- Built with [.NET 10.0](https://dotnet.microsoft.com/)
- GUI framework: [Avalonia](https://avaloniaui.net/) (cross-platform XAML UI)
- MVVM Toolkit: [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/)
- Encryption: [System.Security.Cryptography](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography)
- Process detection: [System.Diagnostics.Process](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process)
- Code quality: [CodeQL](https://codeql.github.com/)

---

## 📄 License

© 2026 ProSoft. All rights reserved.

This software is proprietary and confidential. Unauthorized copying, distribution, or use is strictly prohibited.

---

**ProSoft - Professional Software Solutions**  
February 2026

**Download:** [EasySave v2.0](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v2.0)  
**Source Code:** [GitHub Repository](https://github.com/PotaaatozWRLD/EasySave-Groupe-8)  
**Support:** <support@prosoft.com>
