# EasySave v3.0 - UML Diagrams

This directory contains all UML diagrams for EasySave version 3.0 in PlantUML format.
**Last updated:** February 2026 (v3.0)

---

## 🆕 Version 3.0 Updates

### New Features Reflected in Diagrams:
1.  **Parallel Execution**: `ParallelBackupCoordinator` handling concurrent jobs.
2.  **Priority Management**: Loop logic for processing priority extensions first.
3.  **Large File Throttling**: Semaphore usage for large files in parallel mode.
4.  **Auto-Pause**: `BusinessSoftwareMonitor` interrupting parallel tasks.
5.  **Centralized Logging**: Docker TCP logging via `NetworkLogger`.

### Architecture Changes:
-   **Coordinator Pattern**: `ParallelBackupCoordinator` manages job tasks.
-   **Composite Logger**: Combining local files and network streams.
-   **Mono-Instance**: Mutex usage in separate CryptoSoft process (component diagram).

---

## 📋 Diagrams Included

### 1. **Class Diagram** (`class_diagram.puml`)
Updated with:
-   `ParallelBackupCoordinator`
-   `NetworkLogger` & `CompositeLogger`
-   V3 Configuration fields (`PriorityExtensions`, `MaxLargeFileSizeKB`, `EnableNetworkLogging`)

### 2. **Sequence Diagram - Execute Backup** (`sequence_execute_backup.puml`)
**Major Overhaul**:
-   Shows distinct "Parallel Task" blocks.
-   Illustrates the "Auto-Pause" interrupt cycle.
-   Shows Throttling acquisition/release flow.

### 3. **Sequence Diagram - Create Job** (`sequence_create_job.puml`)
-   Standard job creation flow (Unchanged logic, version bumped).

### 4. **Component Diagram** (`component_diagram.puml`)
Updated with:
-   **Docker Log Server** node.
-   `EasySave.Core` services breakdown.
-   TCP Port 9000 connection.

### 5. **Activity Diagram** (`activity_backup_process.puml`)
Updated workflow:
-   **Parallel Fork/Join** nodes.
-   Priority check loops.
-   Throttling decision points.

### 6. **Use Case Diagram** (`use_case_diagram.puml`)
Added V3 Use Cases:
-   **Execute in Parallel**
-   **Manage Priority Files**
-   **Configure Network Logging**

---

## 🔧 How to Generate PNG Images

**Note**: The `.png` files in this directory may be outdated. Please regenerate them from the `.puml` sources using one of these methods:

### Using PlantUML Online
1.  Go to [PlantUML Web Server](http://www.plantuml.com/plantuml/uml/)
2.  Copy/Paste content from any `.puml` file.
3.  Generate and Save Image.

### Using VS Code Extension
1.  Install "PlantUML" extension (jebbs.plantuml).
2.  Open a `.puml` file.
3.  Press `Alt+D` to preview.
4.  Right-click the preview -> **Export Current Diagram**.

### Using Command Line
```powershell
java -jar plantuml.jar *.puml
```

---

## ✅ Compliance Check (Livrable 3)

| Diagram | Required | Included | Version | Notes |
|---------|----------|----------|---------|-------|
| Class Diagram | ✅ | ✅ | **v3.0** | Full V3 Architecture |
| Sequence Diagram (Execute) | ✅ | ✅ | **v3.0** | Parallel Flow |
| Sequence Diagram (Create) | ✅ | ✅ | **v3.0** | |
| Component Diagram | ✅ | ✅ | **v3.0** | With Docker |
| Activity Diagram | ⚠️ | ✅ | **v3.0** | Parallel Algorithm |
| Use Case Diagram | ⚠️ | ✅ | **v3.0** | V3 Scenarios |

---

## 📧 Contact
**From:** ProSoft - Groupe 8
**To:** Management
**Subject:** Livrable 3 - UML Update
