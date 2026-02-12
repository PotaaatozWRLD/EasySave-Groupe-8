# EasySave - Professional Backup Solution

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/GUI-Avalonia-blue.svg)](https://avaloniaui.net/)
[![Build and Test](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/dotnet.yml/badge.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/codeql.yml/badge.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/codeql.yml)
[![codecov](https://codecov.io/gh/PotaaatozWRLD/EasySave-Groupe-8/graph/badge.svg)](https://codecov.io/gh/PotaaatozWRLD/EasySave-Groupe-8)
[![License](https://img.shields.io/badge/License-ProSoft-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-110%2B%20passing-brightgreen.svg)](EasySave/EasySave.Tests)
[![Release](https://img.shields.io/badge/Release-v2.0-success.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v2.0)

> **EasySave** is a powerful professional backup application developed by ProSoft. Features a modern graphical interface (GUI) and command-line tools, unlimited backup jobs, automatic file encryption, and intelligent business software detection.

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
- **Recursive directory processing** with subdirectories
- Support for local, external, and network paths (UNC)
- Job status tracking with visual indicators

### User Interface (Dual Mode)

#### GUI (Graphical, Recommended)
- **Modern Avalonia UI** with MVVM architecture
- Job list with real-time status badges
- Interactive job editor with validation
- Live progress monitoring (speed, ETA, pause/resume)
- Settings panel for configuration
- Cross-platform capable (Windows, Linux, macOS)

#### Console (CLI, Automation)
- Interactive menu with language selection
- Command-line automation for scripts and schedules
- Backward compatible with v1.0/v1.1

### Encryption & Security (NEW v2.0)

- **AES-256 encryption** for sensitive files
- **Selective by extension**: Encrypt only .docx, .xlsx, .pdf, etc.
- **Per-job configuration**: Different encryption per backup
- **Transparent**: Works seamlessly alongside backup

### Business Software Detection (NEW v2.0)

- **Automatic prevention**: Blocks backup if Word, Excel, SQL Server running
- **Prevents corruption**: Protects files locked by business applications
- **Customizable list**: Add your own applications to monitor
- **Smart scheduling**: Suggests closing software and retrying

### Logging & Monitoring

- **Daily JSON or XML logs** with detailed transfer info
- **Real-time state file** tracking backup progress
- **Encryption metrics**: Tracks encryption time per file
- **UNC path format** for network compatibility
- Logs stored in: `%AppData%\ProSoft\EasySave\Logs\`

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

- **JobManager**: Manages unlimited backup jobs
- **BackupService**: Executes backups with progress tracking
- **EncryptionService**: AES-256 file encryption (NEW v2.0)
- **BusinessSoftwareDetector**: Monitors running applications (NEW v2.0)
- **EasyLog.dll**: Reusable logging library (JSON/XML)
- **PathHelper**: Converts paths to UNC format
- **LanguageManager**: Multi-language support (EN/FR)
- **ViewModels**: MVVM architecture for GUI (NEW v2.0)

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
- ✅ Encryption service (AES-256 encrypt/decrypt)
- ✅ Business software detection
- ✅ MVVM view model binding
- ✅ Configuration management
- ✅ Logging (JSON/XML)
- ✅ Multi-language support
- ✅ Path conversion (UNC format)

---

## 📚 Documentation

- [User Manual v2.0](Documentation/USER_MANUAL.md) - GUI and console usage
- [Technical Support v2.0](Documentation/TECHNICAL_SUPPORT.md) - Configuration and troubleshooting
- [Release Notes v2.0](Documentation/RELEASE_NOTES_v2.0.md) - Latest features
- [Release Notes v1.1](Documentation/RELEASE_NOTES_v1.1.md) - XML logging
- [Release Notes v1.0](Documentation/RELEASE_NOTES_v1.0.md) - Initial release
- [UML Diagrams](UML/) - Architecture documentation

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

### Version 3.0 (Coming Soon)

- Parallel backup execution with priorities
- Large file optimization (chunking >1GB)
- Bandwidth throttling for network drives
- Docker-based log centralization
- Built-in job scheduler
- CryptoSoft mono-instance (mutex protection)

---

## 📄 License

© 2026 ProSoft. All rights reserved.

---

**ProSoft - Professional Software Solutions**
