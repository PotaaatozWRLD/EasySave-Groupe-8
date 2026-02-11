# EasySave v2.0 - UML Diagrams

This directory contains all UML diagrams for EasySave version 2.0 in PlantUML format.
**Last updated:** February 11, 2026 (v2.0)

---

## 🆕 Version 2.0 Updates

### New Features Reflected in Diagrams:
1. **Unlimited Jobs**: Removed 5-job limit (JobManager)
2. **Encryption Support**: Integration with CryptoSoft.exe for priority file extensions
3. **Business Software Detection**: Prevents backups when specific software is running
4. **Dual Logging**: XML and JSON formats with LoggerFactory pattern
5. **GUI Interface**: WPF/Avalonia graphical interface (EasySave.GUI)
6. **Enhanced Job Model**: Added CreatedDate, LastExecutionDate, Description

### Architecture Changes:
- Factory pattern for logger creation (LoggerFactory)
- Service for encryption (EncryptionService)
- Business software detector (BusinessSoftwareDetector)
- MVVM pattern for GUI (MainViewModel)

---

## 📋 Diagrams Included

### 1. **Class Diagram** (`class_diagram.puml`)
Complete class diagram showing:
- All classes with attributes and methods
- Relationships between classes
- Package organization (Console, Core, EasyLog)
- Design patterns (Singleton for LanguageManager)

**Key Components (v2.0):**
- Presentation Layer: Program, LanguageManager, AppConfig, MainViewModel (GUI)
- Business Logic: JobManager (unlimited jobs), BackupService, PathHelper, EncryptionService, BusinessSoftwareDetector
- Logging Library: ILogger, JsonLogger, XmlLogger, LoggerFactory, LogEntry, StateEntry
- Data Models: BackupJob, BackupType, JobState, LogFormat

---

### 2. **Sequence Diagram - Execute Backup** (`sequence_execute_backup.puml`)
Shows the complete flow for executing a backup:
- User initiates backup
- Job retrieval from JobManager
- **v2.0: Business software detection check**
- File processing (with Full/Differential logic)
- **v2.0: Encryption logic for priority files**
- Progress tracking and state updates
- UNC path conversion
- Logging of operations (including EncryptionTime)
- Recursive directory processing

**Actors:** User, Program, JobManager, BackupService, BusinessSoftwareDetector, EncryptionService, PathHelper, ILogger, FileSystem

---

### 3. **Sequence Diagram - Create Job** (`sequence_create_job.puml`)
Details the job creation process:
- User input collection (name, source, target, type)
- **v2.0: No job limit (unlimited jobs)**
- Job persistence to JSON
- Directory creation if needed
- Success/error handling

**Actors:** User, Program, Console, JobManager, FileSystem

---

### 4. **Component Diagram** (`component_diagram.puml`)
Illustrates the system architecture:
- Application components (Console, GUI, Core, EasyLog.dll)
- **v2.0: CryptoSoft.exe external component**
- **v2.0: GUI component (WPF/Avalonia)**
- External dependencies (File System, User)
- Configuration files location (%AppData%)
- Component interactions
- Deployment on .NET Runtime

**Key Aspects:**
- Modular architecture
- Reusable EasyLog.dll
- **v2.0: Encryption and business software detection services**
- **v2.0: LoggerFactory for JSON/XML logging**
- Standard Windows storage locations

---

### 5. **Activity Diagram - Backup Process** (`activity_backup_process.puml`)
Complete backup workflow:
- CLI vs Interactive mode
- **v2.0: Business software detection check**
- Pre-calculation of total files/size
- File-by-file processing
- **v2.0: Encryption logic for priority extensions**
- Progress calculation
- Full vs Differential logic
- Error handling
- Recursive directory traversal
- State management

**Phases:**
1. Initialization
2. **v2.0: Pre-execution checks**
3. Pre-calculation
4. Processing (with progress tracking and encryption)
5. Completion

---

### 6. **Use Case Diagram** (`use_case_diagram.puml`)
User interactions with the system:
- Job Management (Create, List, Edit, Delete - **v2.0: unlimited jobs**)
- Backup Execution (Single, All, CLI)
- Backup Types (Full, Differential - **v2.0: with encryption**)
- Configuration (Language, **v2.0: Log Format**)
- Monitoring (Progress, Logs, State)
- **v2.0: Settings & Configuration (Encryption, Business Software, Extensions)**
- **v2.0: GUI Interface**

**Actors:** User, System Administrator

---

## 🔧 How to Generate PNG Images

### Using PlantUML Online
1. Go to http://www.plantuml.com/plantuml/uml/
2. Copy the content of each `.puml` file
3. Paste and generate
4. Download as PNG

### Using PlantUML Locally

**Install PlantUML:**
```bash
# Windows (with Chocolatey)
choco install plantuml

# Or download JAR from https://plantuml.com/download
```

**Generate all diagrams:**
```bash
cd UML
java -jar plantuml.jar *.puml
```

This will create PNG files for each diagram.

### Using VS Code Extension
1. Install "PlantUML" extension
2. Open any `.puml` file
3. Press `Alt+D` to preview
4. Right-click → Export to save as PNG

---

## 📦 Deliverable Requirements

According to the specifications:
> **Veille Livrable 1 : Livraison des diagrammes UML**

All diagrams must be delivered **24 hours before** the Livrable 1 deadline.

---

## 📝 Diagram Conventions

- **PlantUML format** for easy version control
- **English naming** for all elements (international team requirement)
- **Comprehensive notes** explaining key behaviors
- **Clear relationships** between components
- **Cahier des charges compliance** notes

---

## ✅ Compliance Check

| Diagram | Required | Included | Version | Last Updated | Notes |
|---------|----------|----------|---------|--------------|-------|
| Class Diagram | ✅ | ✅ | **v2.0** | Feb 11, 2026 | Complete architecture with all v2.0 features |
| Sequence Diagram (Execute) | ✅ | ✅ | **v2.0** | Feb 11, 2026 | Encryption & business software check |
| Sequence Diagram (Create) | ✅ | ✅ | **v2.0** | Feb 11, 2026 | Unlimited jobs |
| Component Diagram | ✅ | ✅ | **v2.0** | Feb 11, 2026 | GUI + CryptoSoft integration |
| Activity Diagram | ⚠️ | ✅ | **v2.0** | Feb 11, 2026 | Complete workflow with v2.0 checks |
| Use Case Diagram | ⚠️ | ✅ | **v2.0** | Feb 11, 2026 | All v2.0 use cases documented |

**Legend:**
- ✅ Required by specifications
- ⚠️ Recommended (included for completeness)

---

## 📚 Version History

### v2.0 (February 11, 2026)
- ✅ Added encryption support with CryptoSoft.exe
- ✅ Added business software detection
- ✅ Removed 5-job limitation (unlimited jobs)
- ✅ Added GUI with Avalonia framework
- ✅ Enhanced BackupJob model with dates and description
- ✅ Updated all 6 diagrams to reflect v2.0 architecture
- ✅ Extended AppConfig with encryption and business software settings

### v1.1 (February 8, 2026)
- ✅ Added XML logging format support
- ✅ Added LoggerFactory pattern
- ✅ Extended AppConfig with log format management

### v1.0 (February 5, 2026)
- ✅ Initial release
- ✅ 6 UML diagrams (class, sequence×2, component, activity, use case)
- ✅ JSON logging support
- ✅ Full/Differential backup types
- ✅ Maximum 5 jobs

---

## 📧 Contact

For questions about these diagrams:
- Project: EasySave v2.0
- Company: ProSoft
- Team: Groupe 8

---

**Generated for Livrable 2 - EasySave Version 2.0**  
**Date:** February 11, 2026  
**Team:** ProSoft - Groupe 8  
**Contributors:** Axel Ruffin, Jalil Lalouani, Kenan HUREMOVIC
