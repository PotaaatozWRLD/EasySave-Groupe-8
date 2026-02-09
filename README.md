# EasySave - Professional Backup Solution

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Build and Test](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/dotnet.yml/badge.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/codeql.yml/badge.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions/workflows/codeql.yml)
[![codecov](https://codecov.io/gh/PotaaatozWRLD/EasySave-Groupe-8/graph/badge.svg)](https://codecov.io/gh/PotaaatozWRLD/EasySave-Groupe-8)
[![License](https://img.shields.io/badge/License-ProSoft-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/Tests-82%20passing-brightgreen.svg)](EasySave/EasySave.Tests)
[![Release](https://img.shields.io/badge/Release-v1.0-success.svg)](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v1.0)

> **EasySave** is a powerful console-based backup application developed by ProSoft. It provides reliable file backup with full and differential modes, multi-language support, and comprehensive logging.

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

- **Up to 5 backup jobs** with unique configurations
- **Full Backup**: Complete copy of all files
- **Differential Backup**: Only modified files since last backup
- **Recursive directory processing** with subdirectories
- Support for local drives, external drives, and network paths

### Execution Modes

- **Interactive Console**: User-friendly menu with language selection
- **Command-Line Interface**: Automated execution for scripting
- **Sequential execution** of all configured jobs

### Logging & Monitoring

- **Daily JSON log files** with detailed transfer information
- **Real-time state file** tracking backup progress
- **UNC path format** for network compatibility
- Logs stored in: `%AppData%\ProSoft\EasySave\Logs\`

### Multi-Language Support

- English and French interfaces
- Persistent language preference
- Easy language switching

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

### Interactive Mode

```powershell
.\EasySave.Console.exe
```

### Command-Line Mode

```bash
# Single job
.\EasySave.Console.exe 1

# Range of jobs
.\EasySave.Console.exe 1-3

# Specific jobs
.\EasySave.Console.exe 1;3;5
```

---

## 🏗️ Architecture

### Project Structure

```
EasySave/
├── EasySave.Console/    # Console application
├── EasySave.Core/       # Business logic
├── EasySave.Shared/     # Common models
├── EasyLog/             # Logging library
└── EasySave.Tests/      # Unit tests (82 tests)
```

### Key Components

- **JobManager**: Manages up to 5 backup jobs
- **BackupService**: Executes backups with progress tracking
- **EasyLog.dll**: Reusable logging library
- **PathHelper**: Converts paths to UNC format
- **LanguageManager**: Multi-language support (EN/FR)

---

## 🧪 Testing

The project includes **82 passing tests** covering all core components.

```bash
cd EasySave/EasySave.Tests
dotnet test
```

---

## 📚 Documentation

- [User Manual](Documentation/USER_MANUAL.md)
- [Technical Support](Documentation/TECHNICAL_SUPPORT.md)
- [Release Notes](Documentation/RELEASE_NOTES_v1.0.md)
- [UML Diagrams](UML/)

---

## 👥 Development Team

**ProSoft - Groupe 8**

- **Axel Ruffin** ([@Axelruffin-69](https://github.com/Axelruffin-69))
- **Jalil Lalouani** ([@Maszy69](https://github.com/Maszy69))
- **Kenan HUREMOVIC** ([@PotaaatozWRLD](https://github.com/PotaaatozWRLD))

---

## 🔮 Roadmap

### Version 2.0 (Coming Soon)

- Graphical user interface (WPF/Avalonia)
- Unlimited backup jobs
- File encryption support

### Version 3.0 (Future)

- Parallel backup execution
- Priority file management
- Docker-based log centralization

---

## 📄 License

© 2026 ProSoft. All rights reserved.

---

**ProSoft - Professional Software Solutions**
