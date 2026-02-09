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

## ðŸ“‹ Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Usage](#-usage)
- [Architecture](#-architecture)
- [Testing](#-testing)
- [Documentation](#-documentation)
- [Team](#-development-team)
- [License](#-license)

---

## âœ¨ Features

### Backup Management

- **Up to 5 backup jobs** with unique configurations
- **Full Backup**: Complete copy of all files
- **Differential Backup**: Only modified files since last backup
- **Recursive directory processing** with subdirectories
- Support for local drives, external drives, and network paths

### Execution Modes

- **Interactive Console**: User-friendly menu with language selection
- **Command-Line Interface**: Automated execution for scripting

  ```bash
  EasySave.Console.exe 1        # Run job 1
  EasySave.Console.exe 1-3      # Run jobs 1 through 3
  EasySave.Console.exe 1;3;5    # Run jobs 1, 3, and 5
  EasySave.Console.exe --logs   # Open logs folder
  ```

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

## ðŸš€ Installation

### Prerequisites

- Windows 10 or later
- .NET 10.0 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))

### Option 1: Quick Start (Recommended)

1. **Download** the latest release from [Releases](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases)
2. **Extract** the ZIP archive to your desired location (e.g., `C:\Program Files\EasySave\`)
3. **Navigate** to the extracted folder
4. **Double-click** `EasySave.Console.exe` in Windows Explorer, or run from command prompt:

   ```powershell
   cd "C:\Path\To\EasySave"
   .\EasySave.Console.exe
   ```

### Option 2: Running from Source (Development)

If you have the source code:

```powershell
# Navigate to the Console project folder
cd "c:\Users\YourName\Desktop\Projet Genie Logiciel\EasySave\EasySave.Console"

# Option A: Run directly with dotnet (recommended for development)
dotnet run

# Option B: Build and run the executable
dotnet build -c Debug
cd bin\Debug\net10.0
.\EasySave.Console.exe

# Option C: Build Release version
cd ..\..
 dotnet build -c Release
cd bin\Release\net10.0
.\EasySave.Console.exe
```

### Option 3: Building from GitHub

```powershell
git clone https://github.com/PotaaatozWRLD/EasySave-Groupe-8.git
cd EasySave-Groupe-8\EasySave\EasySave.Console

# Run directly
dotnet run

# OR build and run executable
dotnet build -c Release
cd bin\Release\net10.0
.\EasySave.Console.exe
EasySave.Console.exe
```

---

## ðŸ“– Usage

### Interactive Mode

**For Release version (downloaded from GitHub Releases):**

```powershell
# Option 1: Double-click EasySave.Console.exe in Windows Explorer
# Option 2: Open PowerShell in the folder and run:
.\EasySave.Console.exe
```

**For Development (running from source code):**

```powershell
# Navigate to the project folder
cd "C:\Path\To\EasySave\EasySave.Console"

# Run with dotnet
dotnet run

# OR navigate to the built executable
cd bin\Debug\net10.0
.\EasySave.Console.exe
```

**From any location (if added to PATH):**

```bash
EasySave.Console.exe
```

**Menu Options:**

1. List all backup jobs
2. Create a new backup job
3. Edit an existing job
4. Delete a job
5. Run specific job(s)
6. Run all jobs sequentially
7. Open logs folder
8. Change language
9. Exit

### Command-Line Mode

Execute backups automatically without user interaction:

**Examples:**

```bash
# Navigate to the folder containing EasySave.Console.exe first
cd C:\Path\To\EasySave

# Single job
.\EasySave.Console.exe 1

# Range of jobs
.\EasySave.Console.exe 1-3

# Specific jobs
.\EasySave.Console.exe 1;3;5

# Open logs folder
.\EasySave.Console.exe --logs
# or
.\EasySave.Console.exe -l
```

**For automation (scripts, Task Scheduler):**

```powershell
# Full path in PowerShell script
& "C:\Program Files\EasySave\EasySave.Console.exe" 1-3

# Or in batch file (.bat)
"C:\Program Files\EasySave\EasySave.Console.exe" 1-3
```

### Configuration Files

All configuration and log files are stored in:

```
%AppData%\ProSoft\EasySave\
â”œâ”€â”€ jobs.json          # Backup job configurations
â”œâ”€â”€ config.json        # Application settings
â”œâ”€â”€ state.json         # Real-time backup state
â””â”€â”€ Logs\
    â””â”€â”€ YYYY-MM-DD.json  # Daily log files
```

---

## ðŸ—ï¸ Architecture

### Project Structure

```
EasySave/
â”œâ”€â”€ EasySave.Console/      # Console application (UI)
â”œâ”€â”€ EasySave.Core/         # Business logic
â”‚   â”œâ”€â”€ Services/          # BackupService, JobManager
â”‚   â””â”€â”€ Helpers/           # PathHelper (UNC conversion)
â”œâ”€â”€ EasySave.Shared/       # Common data models
â”œâ”€â”€ EasyLog/               # Logging library (DLL)
â””â”€â”€ EasySave.Tests/        # Unit tests (xUnit)
```

### Key Components

- **JobManager**: Manages up to 5 backup jobs with JSON persistence
- **BackupService**: Executes backups with progress tracking
- **EasyLog.dll**: Reusable logging library for other ProSoft projects
- **PathHelper**: Converts paths to UNC format for network compatibility
- **LanguageManager**: Handles multi-language support (EN/FR)

### Design Patterns

- **Singleton**: LanguageManager for global language state
- **Dependency Injection**: ILogger interface for flexible logging
- **Repository Pattern**: JobManager for job persistence

---

## ðŸ§ª Testing

The project includes a comprehensive unit test suite with **82 tests** covering all core components.

### Running Tests

```bash
cd EasySave/EasySave.Tests
dotnet test
```

### Test Coverage

- **BackupJob, LogEntry, StateEntry**: Data model tests (11 tests)
- **JsonLogger & Logging**: Log generation, UNC paths, error handling (7 tests)
- **JobManager**: Job management with 5-job limit (6 tests)
- **PathHelper**: UNC path conversion (8 tests)
- **LanguageManager**: Multi-language support (7 tests)
- **BackupService**: Integration tests for Full/Differential backups (6 tests)
- **CLI Arguments**: Command-line parsing (1-3, 1;3;5, --logs) (8 tests)
- **Performance**: Large files, progress tracking (6 tests)
- **Error Handling**: Invalid paths, permissions, edge cases (10 tests)

### Test Results

```
Total: 83 tests
Passed: 82 âœ…
Failed: 0
Skipped: 1 (OS-dependent)
Duration: ~5s
```

---

## ðŸ“š Documentation

- **[User Manual](Documentation/USER_MANUAL.md)**: Quick start guide (1 page)
- **[Technical Support](Documentation/TECHNICAL_SUPPORT.md)**: Troubleshooting and configuration
- **[Release Notes](Documentation/RELEASE_NOTES_v1.0.md)**: Version 1.0 features and changes
- **[UML Diagrams](UML/)**: Architecture diagrams (Class, Sequence, Component, Activity, Use Case)

---

## ðŸ‘¥ Development Team

**ProSoft - Groupe 8**

- **Axel Ruffin** ([@Axelruffin-69](https://github.com/Axelruffin-69))
  - Core features & User documentation
  - Data model unit testing

- **Jalil Lalouani** ([@Maszy69](https://github.com/Maszy69))
  - System architecture & UML diagrams
  - Service layer unit testing

- **Kenan HUREMOVIC** ([@PotaaatozWRLD](https://github.com/PotaaatozWRLD))
  - Job management & CLI automation
  - i18n system & integration testing

---

## ðŸ”® Roadmap

### Version 2.0 (Coming Soon)

- Graphical user interface (WPF/Avalonia)
- Unlimited number of backup jobs
- File encryption support (CryptoSoft integration)
- Business software detection (automatic pause)
- XML log format option

### Version 3.0 (Future)

- Parallel backup execution
- Priority file management
- Bandwidth throttling
- Docker-based log centralization

---

## ðŸ’¼ Commercial Information

- **Price**: 200 â‚¬HT per license
- **Maintenance**: 12% annual (24 â‚¬HT/year)
- **Support**: Monday-Friday, 8:00 AM - 5:00 PM
- **Updates**: Included with maintenance contract

---

## ðŸ“ž Support

For technical assistance or bug reports:

- **Email**: <support@prosoft.com>
- **GitHub Issues**: [Report a bug](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/issues)
- **Documentation**: [Technical Support Guide](Documentation/TECHNICAL_SUPPORT.md)

---

## ðŸ“„ License

Â© 2026 ProSoft. All rights reserved.

This software is proprietary and confidential. Unauthorized copying, distribution, or use is strictly prohibited.

---

## ðŸ™ Acknowledgments

- Built with [.NET 10.0](https://dotnet.microsoft.com/)
- Tested with [xUnit](https://xunit.net/)
- Diagrams created with [PlantUML](https://plantuml.com/)

---

**ProSoft - Professional Software Solutions**  
February 2026

#    T e s t 

 
 
