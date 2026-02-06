# EasySave v1.0 - Professional Backup Solution

**Release Date:** February 6, 2026  
**Development Team:** ProSoft - Groupe 8

---

## 🎉 First Official Release

EasySave v1.0 is a professional console-based backup application with comprehensive testing, automated CI/CD, and complete documentation.

---

## 👥 Development Team

- **Kenan HUREMOVIC** ([@PotaaatozWRLD](https://github.com/PotaaatozWRLD))
  - Job management, CLI automation & internationalization
  - Integration testing (7 tests)
  
- **Axel Ruffin** ([@Axelruffin-69](https://github.com/Axelruffin-69))
  - Core features & user documentation
  - Data model testing (11 tests)
  
- **Jalil Lalouani** ([@Maszy69](https://github.com/Maszy69))
  - System architecture & UML diagrams
  - Service layer & performance testing (20 tests)

---

## ✨ Key Features

### 📁 Backup Management

- **Up to 5 backup jobs** with unique configurations
- **Full Backup:** Complete copy of all files
- **Differential Backup:** Only modified files since last backup
- **Recursive processing** with subdirectories support
- Support for **local drives, external drives, and network paths (UNC)**

### ⚙️ Execution Modes

- **Interactive Console:** User-friendly menu with language selection (EN/FR)
- **CLI Automation:** Command-line execution for scripting

  ```bash
  EasySave.Console.exe 1        # Run job 1
  EasySave.Console.exe 1-3      # Run jobs 1 through 3
  EasySave.Console.exe 1;3;5    # Run jobs 1, 3, and 5
  EasySave.Console.exe --logs   # Open logs folder
  ```

- **Sequential execution** of all configured jobs

### 📊 Logging & Monitoring

- **Daily JSON log files** with detailed transfer information
- **Real-time state file** tracking backup progress with percentage
- **UNC path format** for network compatibility
- Logs stored in: `%AppData%\ProSoft\EasySave\Logs\`
- Negative transfer time on errors for easy identification

### 🌍 Multi-Language Support

- **English** and **French** interfaces
- Persistent language preference
- Easy language switching from menu

---

## 🧪 Comprehensive Testing Suite

**82 tests passing** (+ 1 skipped) covering all core components:

### Test Categories

#### Data Models (11 tests)

- BackupJob validation and serialization
- LogEntry with UNC paths and error handling
- StateEntry with real-time progress tracking

#### Logging System (7 tests)

- Daily log file generation (YYYY-MM-DD.json format)
- UNC path conversion and formatting
- Error logging with negative timestamps
- State updates with progress percentages

#### Job Management (6 tests)

- 5-job limit enforcement
- CRUD operations (Create, Read, Update, Delete)
- JSON persistence and validation

#### Path Handling (8 tests)

- UNC path conversion (C:\ → \\\\COMPUTERNAME\\C$\\)
- Network share support
- Special characters handling

#### Internationalization (7 tests)

- English/French language switching
- Persistent configuration
- Fallback to English

#### Integration Tests (6 tests)

- Full backup with multiple files
- Differential backup (only modified files)
- Special characters in filenames
- Recursive subdirectory processing
- Large file handling (5-10 MB)

#### CLI Argument Parsing (8 tests)

- Single job: `1`
- Range: `1-3`
- List: `1;3;5`
- Flags: `--logs`, `-l`
- Invalid input handling

#### Performance Tests (6 tests)

- 100 files backup completion time
- Large file progress tracking (10 MB)
- Mixed file sizes handling
- Transfer time accuracy

#### Error Handling (10 tests)

- Non-existent source paths
- Empty directories
- Read-only files
- Hidden and system files
- Zero-byte files
- Locked files
- Very long filenames

**Test Execution:**

```bash
cd EasySave/EasySave.Tests
dotnet test
# Result: Total: 83 | Passed: 82 ✅ | Failed: 0 | Skipped: 1 | Duration: ~5s
```

---

## 🚀 CI/CD with GitHub Actions

**Automated build and testing** on every push:

- ✅ Compile all 5 projects (EasyLog, Shared, Core, Console, Tests)
- ✅ Run 82 unit tests automatically
- ✅ Generate test reports
- ✅ Badge status in README (passing/failing)

**Workflow:** `.github/workflows/dotnet.yml`

View build status: [GitHub Actions](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/actions)

---

## 🔧 Technical Stack

- **.NET 10.0** framework
- **C#** with Microsoft conventions (PascalCase, camelCase, XML docs)
- **JSON** serialization with indented formatting (human-readable)
- **xUnit** testing framework (82 tests)
- **Modular architecture** with reusable EasyLog.dll
- **AppData storage** for server compatibility

### Project Structure

```
EasySave/
├── EasySave.Console/      # Console UI (Interactive + CLI)
├── EasySave.Core/         # Business logic (BackupService, JobManager)
├── EasySave.Shared/       # Common data models
├── EasyLog/               # Logging library (DLL - version compatible)
└── EasySave.Tests/        # Unit tests (82 tests)
```

### Design Patterns

- **Singleton:** LanguageManager for global state
- **Dependency Injection:** ILogger interface
- **Repository Pattern:** JobManager for persistence

---

## 📦 What's Included

### Executable

- ✅ `EasySave.Console.exe` for Windows 10+ (.NET 10.0 required)

### Documentation

- ✅ **User Manual** (1 page quick start guide)
- ✅ **Technical Support Guide** (troubleshooting, file locations)
- ✅ **Release Notes** (this document)
- ✅ **GPG Setup Guide** (for verified commits)
- ✅ **Professional README.md** with badges, usage examples, and architecture

### UML Diagrams

- ✅ Class Diagram
- ✅ Sequence Diagrams (backup operations)
- ✅ Component Diagram
- ✅ Activity Diagram (backup workflow)
- ✅ Use Case Diagram

### Source Code

- ✅ Complete C# source code with XML documentation
- ✅ 82 unit tests with xUnit
- ✅ GitHub Actions workflow for CI/CD
- ✅ Language files (EN/FR)

---

## 🚀 Installation & Usage

### Quick Start

1. **Download** the latest release from [Releases](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v1.0)
2. **Extract** the ZIP archive
3. **Double-click** `EasySave.Console.exe` or run from PowerShell:

   ```powershell
   .\\EasySave.Console.exe
   ```

### From Source

```powershell
cd EasySave\\EasySave.Console
dotnet run
```

### Command-Line Automation

```powershell
# Single job
.\\EasySave.Console.exe 1

# Multiple jobs
.\\EasySave.Console.exe 1-3

# Specific jobs
.\\EasySave.Console.exe 1;3;5

# Open logs
.\\EasySave.Console.exe --logs
```

---

## 📁 Configuration Files

All files stored in: `%AppData%\\ProSoft\\EasySave\\`

```
├── jobs.json          # Backup job configurations (max 5)
├── config.json        # Application settings (language preference)
├── state.json         # Real-time backup state
└── Logs/
    └── YYYY-MM-DD.json  # Daily log files
```

---

## 🔮 Roadmap - Coming in v2.0

### Graphical User Interface

- WPF or Avalonia-based GUI
- MVVM architecture
- Real-time progress visualization

### Enhanced Features

- **Unlimited backup jobs** (no more 5-job limit)
- **File encryption** via CryptoSoft integration
- **Business software detection** (automatic pause)
- **XML log format** option (in addition to JSON)

### Version 3.0 Preview

- **Parallel backup execution** with priority management
- **Bandwidth throttling**
- **Docker-based log centralization**
- **Large file optimization** (>1 GB)

---

## 💼 Commercial Information

- **License Price:** 200 €HT per unit
- **Maintenance:** 12% annual (24 €HT/year)
  - Free updates and bug fixes
  - Priority support
- **Support Hours:** Monday-Friday, 8:00 AM - 5:00 PM
- **Contact:** <support@prosoft.com>

---

## 🐛 Known Issues

**None reported in v1.0**

For bug reports: [GitHub Issues](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/issues)

---

## 📝 Changelog

### v1.0 (2026-02-06) - Initial Release

#### Features

- ✅ Interactive console with menu system
- ✅ CLI automation support (1, 1-3, 1;3;5 formats)
- ✅ Full and Differential backup modes
- ✅ Up to 5 backup jobs
- ✅ Daily JSON logs with UNC paths
- ✅ Real-time state tracking
- ✅ Multi-language support (EN/FR)
- ✅ Logs viewer (--logs/-l flag)

#### Testing

- ✅ 82 comprehensive unit tests
- ✅ Integration tests for backup operations
- ✅ Performance tests (100 files, 10 MB files)
- ✅ Error handling tests (invalid paths, permissions)
- ✅ CLI argument parsing tests

#### CI/CD

- ✅ GitHub Actions workflow
- ✅ Automated build on every push
- ✅ Automated test execution
- ✅ Build status badge in README

#### Documentation

- ✅ User Manual (1 page)
- ✅ Technical Support Guide
- ✅ 6 UML diagrams
- ✅ Professional README with examples
- ✅ GPG setup guide for verified commits

---

## 🙏 Acknowledgments

- Built with [.NET 10.0](https://dotnet.microsoft.com/)
- Tested with [xUnit](https://xunit.net/)
- Diagrams created with [PlantUML](https://plantuml.com/)
- CI/CD powered by [GitHub Actions](https://github.com/features/actions)

---

## 📄 License

© 2026 ProSoft. All rights reserved.

This software is proprietary and confidential. Unauthorized copying, distribution, or use is strictly prohibited.

---

**ProSoft - Professional Software Solutions**  
February 2026

**Download:** [EasySave v1.0](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v1.0)  
**Source Code:** [GitHub Repository](https://github.com/PotaaatozWRLD/EasySave-Groupe-8)  
**Support:** <support@prosoft.com>
