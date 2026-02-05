# EasySave v1.0 - UML Diagrams

This directory contains all UML diagrams for EasySave version 1.0 in PlantUML format.

## 📋 Diagrams Included

### 1. **Class Diagram** (`class_diagram.puml`)
Complete class diagram showing:
- All classes with attributes and methods
- Relationships between classes
- Package organization (Console, Core, EasyLog)
- Design patterns (Singleton for LanguageManager)

**Key Components:**
- Presentation Layer: Program, LanguageManager, AppConfig
- Business Logic: JobManager, BackupService, PathHelper
- Logging Library: ILogger, JsonLogger, LogEntry, StateEntry
- Data Models: BackupJob, BackupType, JobState

---

### 2. **Sequence Diagram - Execute Backup** (`sequence_execute_backup.puml`)
Shows the complete flow for executing a backup:
- User initiates backup
- Job retrieval from JobManager
- File processing (with Full/Differential logic)
- Progress tracking and state updates
- UNC path conversion
- Logging of operations
- Recursive directory processing

**Actors:** User, Program, JobManager, BackupService, PathHelper, ILogger, FileSystem

---

### 3. **Sequence Diagram - Create Job** (`sequence_create_job.puml`)
Details the job creation process:
- User input collection (name, source, target, type)
- Validation (max 5 jobs)
- Job persistence to JSON
- Directory creation if needed
- Success/error handling

**Actors:** User, Program, Console, JobManager, FileSystem

---

### 4. **Component Diagram** (`component_diagram.puml`)
Illustrates the system architecture:
- Application components (Console, Core, EasyLog.dll)
- External dependencies (File System, User)
- Configuration files location (%AppData%)
- Component interactions
- Deployment on .NET Runtime

**Key Aspects:**
- Modular architecture
- Reusable EasyLog.dll
- Standard Windows storage locations

---

### 5. **Activity Diagram - Backup Process** (`activity_backup_process.puml`)
Complete backup workflow:
- CLI vs Interactive mode
- Pre-calculation of total files/size
- File-by-file processing
- Progress calculation
- Full vs Differential logic
- Error handling
- Recursive directory traversal
- State management

**Phases:**
1. Initialization
2. Pre-calculation
3. Processing (with progress tracking)
4. Completion

---

### 6. **Use Case Diagram** (`use_case_diagram.puml`)
User interactions with the system:
- Job Management (Create, List, Edit, Delete)
- Backup Execution (Single, All, CLI)
- Backup Types (Full, Differential)
- Configuration (Language)
- Monitoring (Progress, Logs, State)

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

| Diagram | Required | Included | Notes |
|---------|----------|----------|-------|
| Class Diagram | ✅ | ✅ | Complete architecture |
| Sequence Diagram | ✅ | ✅ | 2 scenarios (execute + create) |
| Component Diagram | ✅ | ✅ | System architecture |
| Activity Diagram | ⚠️ | ✅ | Recommended (included) |
| Use Case Diagram | ⚠️ | ✅ | Bonus (included) |

---

## 📧 Contact

For questions about these diagrams:
- Project: EasySave v1.0
- Company: ProSoft
- Team: Groupe 8

---

**Generated for Livrable 1 - EasySave Version 1.0**
**Date: February 5, 2026**
