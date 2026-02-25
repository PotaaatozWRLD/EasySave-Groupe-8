# Release Notes - EasySave V3.0

**Date:** 2026-02-16
**Version:** 3.0.0

## 🚀 New Features

### 1. Parallel Backup Execution
- **Concurrency:** Backups now run in parallel instead of sequentially.
- **Performance:** Significantly reduced total backup time for multiple jobs.
- **Architecture:** Utilizes `Task`-based asynchronous execution with a new `ParallelBackupCoordinator`.

### 2. Job Control (Pause/Resume/Stop)
- **Interactive Control:** Users can now Pause, Resume, or Stop individual jobs or all jobs at once.
- **Real-time Status:** UI updates to reflect the current state (Active, Paused, Stopped) of each job.
- **Smart Handling:** Pausing a job safely suspends file transfers without data loss.

### 3. Priority File Management
- **Prioritization:** Files with specific extensions (e.g., `.txt`, `.docx`) can be configured to be backed up *before* other files.
- **Configuration:** Priority extensions are customizable via `config.json`.

### 4. Large File Throttling
- **Bandwidth Control:** Large files (size configurable, default > 1MB) now transfer strictly sequentially to prevent disk I/O saturation.
- **Semaphore:** Implemented using `SemaphoreSlim` to ensure only one large file is processed at a time across all concurrent jobs.

### 5. Business Software Auto-Pause
- **Continuous Monitoring:** A background service now continuously monitors for specific business software processes (e.g., "calc", "notepad").
- **Auto-Action:** If detected, all running backups are automatically **Paused**.
- **Auto-Resume:** When the business software closes, backups automatically **Resume** from where they left off.

### 6. CryptoSoft Mono-Instance
- **Process Safety:** Enforced single-instance execution for `CryptoSoft.exe` using a global `Mutex`.
- **Queueing:** EasySave now queues encryption requests to ensure `CryptoSoft` is never called concurrently, preventing errors.

### 7. Centralized Logging (Docker)
- **3 Logging Modes** configurable directly from Settings UI:
  - **Local** — logs written to `%AppData%\ProSoft\EasySave\Logs\` only
  - **Docker** — logs forwarded to Docker TCP server only
  - **Both** — logs written locally **and** forwarded to Docker simultaneously
- **Docker Integration:** Includes `docker-compose.yml` (Alpine + **socat**, multi-connection support).
- **Machine Identification:** Each log entry includes `MachineName` and `UserName` to identify the source.

## 🛠 Technical Improvements
- **MVVM Refactoring:** Major cleanup of `MainViewModel` to support async commands and parallel coordination.
- **Configuration Upgrade:** `AppConfig` extended to support V3.0 features while maintaining backward compatibility.
- **Robust Error Handling:** Enhanced exception handling for network failures and process concurrency.

## 🐛 Bug Fixes
- Fixed an issue where the UI would freeze during long file transfers.
- Resolved potential race conditions in log file writing.

---
*EasySave V3.0 - ProSoft International*
