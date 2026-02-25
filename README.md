# EasySave - Professional Backup Solution

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/GUI-Avalonia-blue.svg)](https://avaloniaui.net/)
[![Build and Test](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/dotnet.yml/badge.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/codeql.yml/badge.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/codeql.yml)
[![codecov](https://codecov.io/gh/PotaaatozWRLD/EasySave-Groupe-8/graph/badge.svg)](https://codecov.io/gh/PotaaatozWRLD/EasySave-Groupe-8)
[![License](https://img.shields.io/badge/License-ProSoft-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-110%2B%20passing-brightgreen.svg)](EasySave/EasySave.Tests)
[![Release](https://img.shields.io/badge/Release-v3.0-success.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v3.0)

> **EasySave** is a powerful professional backup application developed by ProSoft. Features a modern graphical interface (GUI) and command-line tools, unlimited backup jobs, parallel backup execution with priority file management, automatic file encryption, intelligent business software detection, and Docker-based log centralization.

---

## 📋 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Usage](#-usage)
- [Architecture](#-architecture)
- [Testing](#-testing)
- [Documentation](#-documentation)
- [Team](#-development-team)
- [License](#-license)

---

## ✨ Features

### Backup Management

- **Unlimited backup jobs** (no more 5-job limit!)
- **Full & Differential Backup**: Complete or incremental copies
- **Parallel backup execution** (NEW v3.0): Execute multiple jobs simultaneously
- **Priority file management** (NEW v3.0): Extensions can be marked as priority for faster processing
- **Bandwidth optimization** (NEW v3.0): Limit concurrent large file transfers (>n KB configurable)
- **Recursive directory processing** with subdirectories
- Support for local, external, and network paths (UNC)
- Job status tracking with visual indicators

### User Interface (Dual Mode)

#### GUI (Graphical, Recommended)
- **Modern Avalonia UI** with MVVM architecture
- Job list with real-time status badges
- Interactive job editor with validation
- **Real-time job control** (NEW v3.0): Pause/Play/Stop individual jobs or all jobs together
- **Live progress monitoring**: Speed, ETA, pause/resume with percentage for each job
- **Priority file settings** (NEW v3.0): Configure priority extensions per backup job
- **Bandwidth throttling** (NEW v3.0): Set maximum concurrent large file transfer size
- Settings panel for configuration
- Cross-platform capable (Windows, Linux, macOS)

#### Console (CLI, Automation)
- Interactive menu with language selection
- Command-line automation for scripts and schedules
- Backward compatible with v1.0/v1.1

### Encryption & Security (v2.0+)

- **AES-256 encryption** for sensitive files
- **Selective by extension**: Encrypt only .docx, .xlsx, .pdf, etc.
- **Per-job configuration**: Different encryption per backup
- **CryptoSoft mono-instance** (NEW v3.0): Guaranteed single execution via mutex protection
- **Transparent**: Works seamlessly alongside parallel backups

### Business Software Detection (v2.0+)

- **Intelligent pause system** (UPDATED v3.0): Automatically pauses all backup jobs when business software detected
- **Auto-resume**: Automatically resumes backups when software is closed
- **Prevents corruption**: Protects files locked by business applications
- **Customizable list**: Add your own applications to monitor
- **Real-time monitoring**: Continuous application detection during backup execution

### Logging & Monitoring

- **Daily JSON or XML logs** with detailed transfer info
- **Real-time state file** tracking backup progress for each job
- **Encryption metrics**: Tracks encryption time per file
- **UNC path format** for network compatibility
- **Docker log centralization** (NEW v3.0): Optional centralized logging service
- **Flexible log storage** (NEW v3.0): Choose local, centralized, or hybrid logging
- **Multi-user support** (NEW v3.0): Centralized logs track user identity per entry
- Logs stored in: `%AppData%\ProSoft\EasySave\Logs\` (local) or Docker service (centralized)

### Multi-Language Support

- English and French interfaces
- Persistent language preference
- Easy language switching (no restart needed)

---

## 🚀 Installation

### Prerequisites

- Windows 10 or later
- .NET 10.0 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))

### Quick Start

1. Download the latest release from [Releases](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases)
2. Extract the ZIP archive
3. Run `EasySave.Console.exe`

---

## 📖 Usage

### Graphical User Interface (GUI) - Recommended

```powershell
.\EasySave.GUI.exe
```

Launch the modern GUI for:
- Creating and managing backup jobs
- Real-time progress monitoring
- Configuring encryption and detection
- Viewing detailed logs

### Console (Interactive Menu)

```powershell
.\EasySave.Console.exe
```

Interactive menu for backup management (v1.0/v1.1 style).

### Command-Line Mode (Automation)

```bash
# Single job
.\EasySave.Console.exe 1

# Range of jobs
.\EasySave.Console.exe 1-3

# Specific jobs
.\EasySave.Console.exe 1;3;5;10
```

Perfect for:
- Scheduled backups (Windows Task Scheduler)
- CI/CD pipelines
- Automated scripts

---

## 🏗️ Architecture

### Project Structure

```
EasySave/
├── EasySave.GUI/        # Graphical UI (Avalonia/MVVM) - NEW v2.0
├── EasySave.Console/    # Console application
├── EasySave.Core/       # Business logic (backup, encryption, detection)
├── EasySave.Shared/     # Common models and configuration
├── EasyLog/             # Logging library (JSON/XML)
└── EasySave.Tests/      # Unit tests (110+ tests)
```

### Key Components

- **JobManager**: Manages unlimited backup jobs with parallel execution (NEW v3.0)
- **ParallelBackupExecutor** (NEW v3.0): Orchestrates parallel job execution with priority handling
- **BackupService**: Executes backups with progress tracking and real-time controls
- **PriorityFileManager** (NEW v3.0): Ensures priority extensions are processed first
- **BandwidthThrottler** (NEW v3.0): Limits concurrent large file transfers
- **JobController** (NEW v3.0): Real-time pause/play/stop for individual or all jobs
- **EncryptionService**: AES-256 file encryption with CryptoSoft mono-instance wrapper (v3.0)
- **BusinessSoftwareDetector**: Monitors and auto-pauses backups when business software detected (v3.0)
- **LogCentralizationService** (NEW v3.0): Docker integration for centralized logging
- **EasyLog.dll**: Reusable logging library (JSON/XML) with centralization support
- **PathHelper**: Converts paths to UNC format
- **LanguageManager**: Multi-language support (EN/FR)
- **ViewModels**: MVVM architecture for GUI (v2.0+)

### MVVM Architecture (GUI)

```
View (XAML)
    ↓
ViewModel (Logic) → Model (Data)
```

Clean separation of concerns for maintainability and testability.

---

## 🧪 Testing

The project includes **110+ passing tests** covering all components.

```bash
cd EasySave/EasySave.Tests
dotnet test
```

### Test Coverage

- ✅ Backup operations (local, external, network)
- ✅ Parallel backup execution with synchronization
- ✅ Priority file management and queuing
- ✅ Bandwidth throttling for large files
- ✅ Real-time job control (pause/play/stop)
- ✅ Business software auto-pause functionality
- ✅ Encryption service (AES-256 encrypt/decrypt)
- ✅ CryptoSoft mono-instance mutex protection
- ✅ Log centralization (Docker integration)
- ✅ MVVM view model binding
- ✅ Configuration management
- ✅ Logging (JSON/XML) with centralization
- ✅ Multi-language support
- ✅ Path conversion (UNC format)

---

## 📚 Documentation

- [User Manual v3.0](Documentation/USER_MANUAL.md) - GUI, parallel execution, and console usage
- [Technical Support v3.0](Documentation/TECHNICAL_SUPPORT.md) - Configuration, Docker setup, and troubleshooting
- [Release Notes v3.0](Documentation/RELEASE_NOTES_v3.0.md) - Latest features and improvements
- [Release Notes v2.0](Documentation/RELEASE_NOTES_v2.0.md) - GUI and encryption features
- [Release Notes v1.1](Documentation/RELEASE_NOTES_v1.1.md) - XML logging
- [Release Notes v1.0](Documentation/RELEASE_NOTES_v1.0.md) - Initial release
- [UML Diagrams](UML/) - Architecture documentation (updated for v3.0)

---

## 👥 Development Team

**ProSoft - Groupe 8**

- **Axel Ruffin** ([@Axelruffin-69](https://github.com/Axelruffin-69))
- **Jalil Lalouani** ([@Maszy69](https://github.com/Maszy69))
- **Kenan HUREMOVIC** ([@PotaaatozWRLD](https://github.com/PotaaatozWRLD))

---

## 🔮 Roadmap

### Version 2.0 ✅ (Released February 2026)

- ✅ Graphical user interface (Avalonia MVVM)
- ✅ Unlimited backup jobs
- ✅ File encryption (AES-256)
- ✅ Business software detection
- ✅ Dual interface (GUI + Console)

### Version 3.0 ✅ (Released February 2026)

- ✅ Parallel backup execution with real-time job control
- ✅ Priority file management by extension
- ✅ Bandwidth throttling for large file transfers
- ✅ Automatic pause/resume on business software detection
- ✅ Docker-based log centralization (local, centralized, or hybrid modes)
- ✅ Mono-instance CryptoSoft with mutex protection
- ✅ Enhanced progress tracking for parallel jobs
- ✅ Pause/Play/Stop controls for individual or all jobs

### Version 4.0 (Future Considerations)

- Cloud backup support (Azure, AWS, Google Drive)
- File compression and deduplication
- Web interface for centralized management
- Built-in job scheduler with notifications
- Webhook integration for custom alerting
- Real-time dashboard and monitoring
- REST API for enterprise integration
- Advanced analytics and reporting

---

## 📄 License

© 2026 ProSoft. All rights reserved.

---

**ProSoft - Professional Software Solutions**
