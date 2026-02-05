# EasySave - Release Notes

## Version 1.0 - February 5, 2026

### 🎉 Initial Release

EasySave 1.0 is the first release of ProSoft's backup solution. This version provides core backup functionality through a console interface with support for multiple languages.

---

## ✨ New Features

### Backup Management
- **Multiple Backup Jobs**: Create and manage up to 5 backup jobs
- **Two Backup Types**:
  - **Full Backup**: Complete copy of all files from source to target
  - **Differential Backup**: Only copies modified files since last backup
- **Job Configuration**: Each job includes name, source path, target path, and backup type
- **CRUD Operations**: Create, Read, Update, and Delete backup jobs

### Execution Modes
- **Interactive Console Menu**: User-friendly menu for all operations
- **Command Line Interface**: Execute backups via terminal commands
  - Single job: `EasySave.exe 1`
  - Range: `EasySave.exe 1-3`
  - Multiple: `EasySave.exe 1;3;5`
- **Sequential Execution**: Run all configured jobs one after another

### Multi-Language Support
- **English**: Full interface in English
- **French**: Complete French translation
- **Language Selection**: Choose language on startup
- **Persistent Settings**: Language preference saved across sessions

### Logging and Monitoring
- **Daily Log Files**: Automatic creation of JSON log files (one per day)
- **Detailed Logging**: Each file transfer logged with:
  - Timestamp
  - Job name
  - Source and target paths (UNC format)
  - File size
  - Transfer time in milliseconds
  - Error messages (if applicable)
- **Real-Time State File**: Live progress tracking with:
  - Current job status (Active/End/Paused)
  - Total files and size
  - Progress percentage
  - Remaining files and size
  - Current file being processed

### Path Support
- **Local Drives**: C:\, D:\, etc.
- **External Drives**: USB drives, external hard drives
- **Network Paths**: UNC paths (\\server\share)
- **Recursive Backup**: Automatically includes all subdirectories and files

### Architecture
- **Modular Design**: Separated into multiple projects for maintainability
  - `EasySave.Console`: User interface
  - `EasySave.Core`: Business logic
  - `EasyLog.dll`: Logging library (reusable for other projects)
  - `EasySave.Shared`: Common data models
- **DLL Library**: EasyLog packaged as Dynamic Link Library for reuse
- **Clean Code**: English comments and documentation for international teams

---

## 📂 File Locations

### Configuration Files
All configuration and log files are stored in a standard location:
```
%AppData%\ProSoft\EasySave\
```

This ensures proper functioning on client servers and multi-user environments.

### File Structure
- `jobs.json`: Backup job configurations
- `config.json`: Application settings (language)
- `state.json`: Real-time backup progress
- `Logs\YYYY-MM-DD.json`: Daily log files

---

## 🔧 Technical Specifications

### Framework
- **.NET 10.0**: Built with latest .NET framework
- **C#**: Pure C# implementation
- **JSON Format**: All data files use JSON with proper formatting (indented)

### Performance
- **Efficient File Copying**: Uses .NET file streams for optimal performance
- **Progress Tracking**: Real-time calculation of completion percentage
- **Error Handling**: Comprehensive error catching and logging

### Code Quality
- **No Code Duplication**: DRY principles applied throughout
- **Meaningful Names**: Clear and consistent naming conventions
- **Commented Code**: English documentation for all major components
- **Modular Architecture**: Easy to maintain and extend

---

## 📋 Known Limitations

### Job Limit
- Maximum of 5 backup jobs (as per specifications)
- Attempting to create more will result in an error message

### Backup Execution
- Sequential execution only (no parallel processing in this version)
- No pause/resume functionality during backup
- No cancellation once started (file completes before stopping)

### User Interface
- Console-only interface (no graphical UI in version 1.0)
- Menu-driven interaction only

---

## 🐛 Bug Fixes

N/A - Initial release

---

## 📖 Documentation

### Included Documentation
- **User Manual**: Step-by-step guide for end users (1 page)
- **Technical Support Documentation**: Comprehensive support guide
  - Installation locations
  - Configuration file formats
  - Troubleshooting procedures
  - System requirements
- **Code Documentation**: XML comments in source code

---

## 🔮 Future Versions

Version 2.0 is planned with the following enhancements:
- Graphical user interface (WPF/Avalonia)
- Unlimited number of backup jobs
- File encryption support (CryptoSoft integration)
- Business software detection (automatic pause)
- XML log format option

---

## 💼 Commercial Information

- **Price**: 200 €HT per license
- **Maintenance**: 12% annual (24 €HT/year)
- **Support**: Monday-Friday, 8:00 AM - 5:00 PM
- **Updates**: Included with maintenance contract

---

## 🙏 Credits

**Development Team**: ProSoft Engineering Team
**Project**: EasySave Backup Solution
**Release Date**: February 5, 2026

---

## 📞 Support

For technical assistance or bug reports:
- **Email**: support@prosoft.com
- **Phone**: Available during support hours
- **GitHub**: Issue tracking and feature requests

---

**ProSoft - Professional Software Solutions**
© 2026 ProSoft. All rights reserved.
