# EasySave v1.1 - XML Logging & Enhanced Configuration

**Release Date:** February 9, 2026  
**Development Team:** ProSoft - Groupe 8

---

## 🎉 Second Official Release

EasySave v1.1 introduces XML logging format support, enhanced configuration management, and improved code quality based on CodeQL security analysis.

---

## 👥 Development Team

- **Kenan HUREMOVIC** ([@PotaaatozWRLD](https://github.com/PotaaatozWRLD))
  - XML logger implementation & factory pattern
  - Settings menu & hot-swap capability
  
- **Axel Ruffin** ([@Axelruffin-69](https://github.com/Axelruffin-69))
  - Documentation updates & testing coordination
  
- **Jalil Lalouani** ([@Maszy69](https://github.com/Maszy69))
  - Code quality improvements & UML updates

---

## ✨ What's New in v1.1

### 📝 XML Logging Format

- **Dual Format Support:** Choose between JSON (v1.0) or XML (v1.1) log formats
- **Seamless Switching:** Change log format without restarting the application
- **Indented XML:** Readable in Notepad with proper formatting
- **Backward Compatible:** JSON remains the default format

**XML Log Example:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LogEntries>
  <LogEntry>
    <Timestamp>2026-02-09T14:30:25</Timestamp>
    <JobName>Documents Backup</JobName>
    <SourcePath>\\localhost\C$\Documents\file.txt</SourcePath>
    <TargetPath>\\localhost\D$\Backup\file.txt</TargetPath>
    <FileName>file.txt</FileName>
    <FileSize>1024000</FileSize>
    <TransferTime>125</TransferTime>
    <EncryptionTime>0</EncryptionTime>
  </LogEntry>
</LogEntries>
```

### ⚙️ Settings Menu

New **Option 9** in the main menu:

- View current log format (JSON or XML)
- Switch between formats instantly
- No application restart required
- Persistent configuration in `config.json`

**Usage:**

1. Launch EasySave in interactive mode
2. Select **Option 9: Settings**
3. Choose your preferred log format
4. Changes apply immediately to new backups

### 🏭 Factory Pattern Implementation

**LoggerFactory** with `LogFormat` enum:

```csharp
public enum LogFormat { JSON, XML }

ILogger logger = LoggerFactory.CreateLogger(
    format: LogFormat.XML,
    logDirectory: "%AppData%\ProSoft\EasySave\Logs",
    stateFilePath: "%AppData%\ProSoft\EasySave\state.xml"
);
```

**Benefits:**

- Easier to add new formats in future versions
- Cleaner code with strategy pattern
- Type-safe format selection

### 🔄 Hot-Swap Capability

**No Restart Required:**

- Logger instance created dynamically for each backup
- Format change takes effect immediately
- State file extension changes automatically (.json → .xml)
- Previous logs remain untouched

**How it works:**

1. User changes format in Settings menu
2. Next backup creates fresh logger instance
3. New logs written in selected format
4. Old logs remain in original format

---

## 🧪 Enhanced Testing Suite

**93 tests passing** (11 new XML logger tests):

### New Test Categories (v1.1)

#### XmlLogger Tests (11 tests)

- ✅ Daily XML log file creation (YYYY-MM-DD.xml)
- ✅ Valid XML structure validation
- ✅ Indentation for Notepad readability
- ✅ Multiple entries append correctly
- ✅ Error message handling
- ✅ Corrupted file recovery (try-catch)
- ✅ ILogger interface compliance
- ✅ Factory pattern (XML creation)
- ✅ Factory pattern (JSON creation)
- ✅ State file creation with .xml extension
- ✅ State update correctness

**Test Execution:**

```bash
cd EasySave/EasySave.Tests
dotnet test --filter "FullyQualifiedName~XmlLoggerTests"
# Result: Total: 11 | Passed: 11 ✅ | Failed: 0 | Duration: ~1.5s
```

**Full Test Suite:**

```bash
dotnet test
# Result: Total: 93 | Passed: 93 ✅ | Failed: 0 | Duration: ~6s
```

---

## 🔧 Technical Improvements

### Code Quality

- ✅ **CodeQL Security Analysis:** No critical warnings
- ✅ **Ternary Operators:** Cleaner conditional assignments
- ✅ **Specific Exception Handling:** No generic catch blocks
- ✅ **Null-Safety:** Added null checks for CodeQL compliance

### Architecture Enhancements

- ✅ **Dynamic Logger Creation:** Logger instantiated per backup
- ✅ **AppData Configuration:** Consistent use of `%AppData%` paths
- ✅ **Modular Design:** EasyLog.dll remains version-compatible
- ✅ **Static Path Fields:** Reduced redundancy in path declarations

### Performance

- No performance degradation from v1.0
- XML serialization comparable to JSON
- Indentation adds <5% to file size

---

## 📦 What's Updated

### Configuration Files

**config.json** now includes:

```json
{
  "Language": "en",
  "LogFormatString": "JSON"
}
```

**state.xml** (when XML format selected):

```xml
<?xml version="1.0" encoding="utf-8"?>
<StateEntries>
  <StateEntry>
    <Name>Documents Backup</Name>
    <State>ACTIVE</State>
    <Progression>45</Progression>
    <!-- ... -->
  </StateEntry>
</StateEntries>
```

### Language Files

**New translations** for Settings menu:

**English (lang.en.json):**

- `"Settings_Title": "Settings"`
- `"Settings_CurrentLogFormat": "Current log format: {0}"`
- `"Settings_ChangeLogFormat": "Change log format"`
- `"LogFormat_Changed": "Log format changed to {0}"`

**French (lang.fr.json):**

- `"Settings_Title": "Paramètres"`
- `"Settings_CurrentLogFormat": "Format de log actuel : {0}"`
- `"Settings_ChangeLogFormat": "Changer le format de log"`
- `"LogFormat_Changed": "Format de log changé en {0}"`

---

## 🚀 Migration from v1.0

### Automatic Migration

**No action required!**

- v1.1 reads v1.0 configuration files
- JSON remains the default format
- Existing logs remain accessible
- `jobs.json` structure unchanged

### Switching to XML

1. Launch EasySave v1.1
2. Select **Option 9: Settings**
3. Choose **XML** format
4. New backups will create XML logs
5. Old JSON logs remain in `Logs/` folder

### Reverting to JSON

Same process - select JSON format in Settings menu.

---

## 📁 File Locations (v1.1)

All files stored in: `%AppData%\ProSoft\EasySave\`

```
├── config.json              # Application settings (language, log format)
├── jobs.json                # Backup job configurations (max 5)
├── state.json OR state.xml  # Real-time state (format depends on selection)
└── Logs/
    ├── 2026-02-09.json      # Daily logs in JSON format
    └── 2026-02-09.xml       # Daily logs in XML format (if selected)
```

---

## 🔮 Roadmap - Coming in v2.0

### Graphical User Interface

- WPF or Avalonia-based GUI
- MVVM architecture
- Real-time progress visualization
- Drag-and-drop backup configuration

### Enhanced Features

- **Unlimited backup jobs** (no more 5-job limit)
- **File encryption** via CryptoSoft integration
- **Business software detection** (automatic pause)
- **Advanced filtering** (file extensions, size limits)

### Version 3.0 Preview

- **Parallel backup execution** with priority management
- **Bandwidth throttling** for network backups
- **Docker-based log centralization**
- **Large file optimization** (>1 GB with chunking)

---

## 💼 Commercial Information

- **License Price:** 200 €HT per unit
- **Maintenance:** 12% annual (24 €HT/year)
  - Free updates (v1.1 included)
  - Priority support
- **Support Hours:** Monday-Friday, 8:00 AM - 5:00 PM
- **Contact:** <support@prosoft.com>

---

## 🐛 Known Issues

**None reported in v1.1**

For bug reports: [GitHub Issues](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/issues)

---

## 📝 Changelog

### v1.1 (2026-02-09) - XML Logging & Configuration

#### New Features ✨

- ✅ **XML Logging Format:** Alternative to JSON with indented output
- ✅ **Settings Menu (Option 9):** Interactive format selection
- ✅ **Hot-Swap Capability:** Format changes apply immediately
- ✅ **LoggerFactory:** Factory pattern for logger instantiation
- ✅ **Dynamic Logger Creation:** Logger recreated per backup

#### Technical Improvements 🔧

- ✅ **XmlLogger Implementation:** 220 lines with proper error handling
- ✅ **AppData Configuration:** Consistent path usage across all components
- ✅ **State File Extension:** Automatic .xml/.json based on format
- ✅ **Ternary Operators:** Cleaner conditional code
- ✅ **Null-Safety Checks:** CodeQL compliance

#### Testing 🧪

- ✅ **11 New Tests:** XmlLogger comprehensive coverage
- ✅ **93 Total Tests:** All passing (100% success rate)
- ✅ **CodeQL Analysis:** No critical warnings
- ✅ **CI/CD Pipeline:** Automated build and test on every push

#### Documentation 📚

- ✅ **Release Notes v1.1:** Complete changelog
- ✅ **User Manual Updates:** Settings menu usage
- ✅ **Technical Support Updates:** XML format troubleshooting
- ✅ **Language Files:** EN/FR translations for Settings

### v1.0 (2026-02-06) - Initial Release

See [RELEASE_NOTES_v1.0.md](RELEASE_NOTES_v1.0.md) for details.

---

## 🙏 Acknowledgments

- Built with [.NET 10.0](https://dotnet.microsoft.com/)
- XML serialization with [System.Xml.Serialization](https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization)
- Code quality with [CodeQL](https://codeql.github.com/)
- CI/CD powered by [GitHub Actions](https://github.com/features/actions)

---

## 📄 License

© 2026 ProSoft. All rights reserved.

This software is proprietary and confidential. Unauthorized copying, distribution, or use is strictly prohibited.

---

**ProSoft - Professional Software Solutions**  
February 2026

**Download:** [EasySave v1.1](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/releases/tag/v1.1)  
**Source Code:** [GitHub Repository](https://github.com/PotaaatozWRLD/EasySave-Groupe-8)  
**Support:** <support@prosoft.com>
