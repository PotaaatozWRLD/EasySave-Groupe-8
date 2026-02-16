using System.Diagnostics;
using System.IO;
using EasySave.Shared;
using EasyLog;
using EasySave.Core.Helpers;
using EasySave.Core.Models;

namespace EasySave.Core.Services;

/// <summary>
/// Service responsible for executing backup jobs.
/// Handles file copying, progress tracking, and logging.
/// </summary>
public class BackupService
{
    private readonly ILogger _logger;
    private readonly EncryptionService? _encryptionService;
    private readonly List<string> _extensionsToEncrypt;
    private readonly LargeFileThrottle? _largeFileThrottle; // V3.0
    private readonly List<string> _priorityExtensions; // V3.0

    public BackupService(ILogger logger)
    {
        _logger = logger;
        _extensionsToEncrypt = new List<string>();
        _encryptionService = null;
        _largeFileThrottle = null; // No throttling in basic constructor
        _priorityExtensions = new List<string>(); // No priority files
    }
    
    /// <summary>
    /// Initializes BackupService with encryption support (v2.0).
    /// V3.0: Also initializes large file throttling if configured.
    /// </summary>
    /// <param name="logger">Logger instance for writing logs.</param>
    /// <param name="cryptoSoftPath">Path to CryptoSoft.exe executable.</param>
    /// <param name="extensionsToEncrypt">List of file extensions to encrypt (e.g., ".docx", ".xlsx").</param>
    /// <param name="maxLargeFileSizeKB">V3.0: Max file size in KB for throttling (0 = no limit).</param>
    /// <param name="priorityExtensions">V3.0: List of priority file extensions (processed first).</param>
    public BackupService(ILogger logger, string cryptoSoftPath, List<string> extensionsToEncrypt, long maxLargeFileSizeKB = 0, List<string>? priorityExtensions = null)
    {
        _logger = logger;
        _extensionsToEncrypt = extensionsToEncrypt ?? new List<string>();
        _priorityExtensions = priorityExtensions ?? new List<string>();
        
        if (!string.IsNullOrWhiteSpace(cryptoSoftPath) && File.Exists(cryptoSoftPath))
        {
            _encryptionService = new EncryptionService(cryptoSoftPath);
        }
        
        // V3.0: Initialize large file throttling
        if (maxLargeFileSizeKB > 0)
        {
            _largeFileThrottle = new LargeFileThrottle(maxLargeFileSizeKB);
        }
    }

    /// <summary>
    /// Executes a backup job with full progress tracking.
    /// v2.0: Prevents backup if business software is running.
    /// </summary>
    /// <param name="job">The backup job to execute</param>
    /// <param name="businessSoftwareName">Optional name of business software to check (v2.0). If running, backup will be blocked.</param>
    /// <param name="progress">Optional progress reporter for UI updates (0-100)</param>
    /// <exception cref="InvalidOperationException">Thrown when business software is running and backup cannot proceed.</exception>
    public void ExecuteBackup(BackupJob job, string? businessSoftwareName = null, IProgress<(int filesProcessed, int totalFiles)>? progress = null)
    {
        try
        {
            // v2.0: Check if business software is running
            // Support both single name (legacy) and multiple names
            var businessSoftwareList = new List<string>();
            if (!string.IsNullOrWhiteSpace(businessSoftwareName))
            {
                // Check if it's a semicolon-separated list
                businessSoftwareList = businessSoftwareName.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
            }
            
            if (businessSoftwareList.Count > 0 && BusinessSoftwareDetector.IsAnyRunning(businessSoftwareList))
            {
                string softwareList = string.Join(", ", businessSoftwareList);
                throw new InvalidOperationException(
                    $"Cannot start backup '{job.Name}': One or more business software applications ({softwareList}) are currently running. Please close them and try again.");
            }

            // Calculate total files and size before starting
            var (totalFiles, totalSize) = CalculateTotalFilesAndSize(job.SourcePath);
            
            // Initialize state
            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                State = JobState.ACTIVE,
                TotalFiles = totalFiles,
                TotalSize = totalSize,
                Progression = 0,
                NbFilesLeftToDo = totalFiles,
                NbFilesLeftToDoSize = totalSize,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });

            // Start the backup process
            int filesProcessed = 0;
            long bytesProcessed = 0;
            ProcessDirectory(job.SourcePath, job.TargetPath, job, ref filesProcessed, ref bytesProcessed, totalFiles, totalSize, progress);
            
            // Mark as completed
            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                State = JobState.END,
                TotalFiles = totalFiles,
                TotalSize = totalSize,
                Progression = 100,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
        }
        catch (IOException ex)
        {
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                SourcePath = PathHelper.ToUncPath(job.SourcePath),
                TargetPath = PathHelper.ToUncPath(job.TargetPath),
                FileName = "",
                FileSize = 0,
                TransferTime = -1,
                ErrorMessage = ex.Message
            });

            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                State = JobState.END,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalFiles = 0,
                TotalSize = 0,
                Progression = 0,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                SourcePath = PathHelper.ToUncPath(job.SourcePath),
                TargetPath = PathHelper.ToUncPath(job.TargetPath),
                FileName = "",
                FileSize = 0,
                TransferTime = -1,
                ErrorMessage = ex.Message
            });

            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                State = JobState.END,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalFiles = 0,
                TotalSize = 0,
                Progression = 0,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
        }
    }

    /// <summary>
    /// Calculates the total number of files and their total size in a directory tree.
    /// </summary>
    private (int totalFiles, long totalSize) CalculateTotalFilesAndSize(string sourceDir)
    {
        int totalFiles = 0;
        long totalSize = 0;

        try
        {
            foreach (var filePath in Directory.GetFiles(sourceDir))
            {
                totalFiles++;
                var fileInfo = new FileInfo(filePath);
                totalSize += fileInfo.Length;
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var (subFiles, subSize) = CalculateTotalFilesAndSize(subDir);
                totalFiles += subFiles;
                totalSize += subSize;
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore access errors during calculation
        }
        catch (IOException)
        {
            // Ignore I/O errors during calculation
        }

        return (totalFiles, totalSize);
    }

    /// <summary>
    /// Recursively processes a directory and copies files according to the backup type.
    /// V3.0: Supports JobExecutionContext for pause/stop controls.
    /// </summary>
    private void ProcessDirectory(string sourceDir, string targetDir, BackupJob job, 
                                  ref int filesProcessed, ref long bytesProcessed, 
<<<<<<< HEAD
<<<<<<< Updated upstream
                                  int totalFiles, long totalSize)
=======
                                  int totalFiles, long totalSize, IProgress<(int filesProcessed, int totalFiles)>? progress = null,
                                  JobExecutionContext? context = null)
>>>>>>> Stashed changes
=======
                                  int totalFiles, long totalSize, IProgress<(int filesProcessed, int totalFiles)>? progress = null)
>>>>>>> ab063d7db3026d7aa9cf412ec4ae92de3a1d1dfb
    {
        // Ensure the target directory exists
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        // Process all files in the directory
        foreach (var filePath in Directory.GetFiles(sourceDir))
        {
            // V3.0: Check if job should pause or has been cancelled
            context?.CheckPauseAndCancellation();
            
            string fileName = Path.GetFileName(filePath);
            string targetFilePath = Path.Combine(targetDir, fileName);

            try
            {
                // Get file size for progress calculation
                var fileInfo = new FileInfo(filePath);
                long fileSize = fileInfo.Length;

                // Update state before copying
                int progression = totalFiles > 0 ? (int)((filesProcessed * 100.0) / totalFiles) : 0;
                _logger.UpdateState(new StateEntry
                {
                    Name = job.Name,
                    CurrentSourceFilePath = filePath,
                    CurrentTargetFilePath = targetFilePath,
                    State = JobState.ACTIVE,
                    LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalFiles = totalFiles,
                    TotalSize = totalSize,
                    Progression = progression,
                    NbFilesLeftToDo = totalFiles - filesProcessed,
                    NbFilesLeftToDoSize = totalSize - bytesProcessed
                });

                // Perform the copy based on backup type
                if (job.Type == BackupType.Full ||
                    (job.Type == BackupType.Differential && IsSourceNewer(filePath, targetFilePath)))
                {
                    var stopwatch = Stopwatch.StartNew();
                    long encryptionTime = 0;

                    // v2.0: Check if file should be encrypted
                    bool shouldEncrypt = _encryptionService != null && 
                                        EncryptionService.ShouldEncrypt(filePath, _extensionsToEncrypt);

                    if (shouldEncrypt)
                    {
                        // Copy to temporary file first
                        string tempTarget = targetFilePath + ".temp";
                        using (var sourceStream = File.OpenRead(filePath))
                        using (var targetStream = File.Create(tempTarget))
                        {
                            sourceStream.CopyTo(targetStream);
                        }

                        // Encrypt the temporary file
                        encryptionTime = _encryptionService!.EncryptFile(tempTarget, targetFilePath);
                        
                        // Delete temporary file
                        try { File.Delete(tempTarget); } 
                        catch (IOException) { /* Ignore cleanup errors */ }
                        catch (UnauthorizedAccessException) { /* Ignore cleanup errors */ }
                        
                        // If encryption failed, copy the original file
                        if (encryptionTime < 0)
                        {
                            File.Copy(filePath, targetFilePath, true);
                        }
                    }
                    else
                    {
                        // No encryption - direct copy
                        using (var sourceStream = File.OpenRead(filePath))
                        using (var targetStream = File.Create(targetFilePath))
                        {
                            sourceStream.CopyTo(targetStream);
                        }
                    }

                    stopwatch.Stop();

                    // Log the successful copy with encryption time
                    _logger.WriteLog(new LogEntry
                    {
                        JobName = job.Name,
                        SourcePath = PathHelper.ToUncPath(filePath),
                        TargetPath = PathHelper.ToUncPath(targetFilePath),
                        FileName = fileName,
                        FileSize = fileSize,
                        TransferTime = stopwatch.ElapsedMilliseconds,
                        EncryptionTime = encryptionTime, // v2.0: Track encryption time
                        ErrorMessage = null,
                        Timestamp = DateTime.Now
                    });
                }

                // Update progress counters
                filesProcessed++;
                bytesProcessed += fileSize;
                
                // Report progress to UI if callback provided
                progress?.Report((filesProcessed, totalFiles));
            }
            catch (IOException ex)
            {
                // Get file size if possible
                long fileSize = 0;
                try
                {
                    if (File.Exists(filePath))
                    {
                        fileSize = new FileInfo(filePath).Length;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // File access denied during size calculation - ignore
                }
                catch (IOException)
                {
                    // File I/O error during size calculation - ignore
                }

                _logger.WriteLog(new LogEntry
                {
                    JobName = job.Name,
                    SourcePath = PathHelper.ToUncPath(filePath),
                    TargetPath = PathHelper.ToUncPath(targetFilePath),
                    FileName = Path.GetFileName(filePath),
                    FileSize = fileSize,
                    TransferTime = -1,
                    ErrorMessage = ex.Message
                });

                filesProcessed++;
                bytesProcessed += fileSize;
            }
            catch (UnauthorizedAccessException ex)
            {
                // Get file size if possible
                long fileSize = 0;
                try
                {
                    if (File.Exists(filePath))
                    {
                        fileSize = new FileInfo(filePath).Length;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // File access denied during size calculation - ignore
                }
                catch (IOException)
                {
                    // File I/O error during size calculation - ignore
                }

                _logger.WriteLog(new LogEntry
                {
                    JobName = job.Name,
                    SourcePath = PathHelper.ToUncPath(filePath),
                    TargetPath = PathHelper.ToUncPath(targetFilePath),
                    FileName = Path.GetFileName(filePath),
                    FileSize = fileSize,
                    TransferTime = -1,
                    ErrorMessage = ex.Message
                });

                filesProcessed++;
                bytesProcessed += fileSize;
            }
        }

        // Recursively process subdirectories
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            string subDirName = Path.GetFileName(subDir);
            string targetSubDir = Path.Combine(targetDir, subDirName);
<<<<<<< HEAD
<<<<<<< Updated upstream
            ProcessDirectory(subDir, targetSubDir, job, ref filesProcessed, ref bytesProcessed, totalFiles, totalSize);
=======
            ProcessDirectory(subDir, targetSubDir, job, ref filesProcessed, ref bytesProcessed, totalFiles, totalSize, progress, context);
        }
    }

    /// <summary>
    /// Executes a backup job with V3.0 JobExecutionContext support (pause/stop controls).
    /// This method is used by ParallelBackupCoordinator for parallel execution.
    /// </summary>
    /// <param name="job">The backup job to execute</param>
    /// <param name="businessSoftwareName">Optional business software to check</param>
    /// <param name="progress">Progress reporter for UI updates</param>
    /// <param name="context">Execution context for pause/stop controls</param>
    public void ExecuteBackupWithContext(
        BackupJob job,
        string? businessSoftwareName,
        IProgress<(int filesProcessed, int totalFiles)> progress,
        JobExecutionContext context)
    {
        try
        {
            // V2.0: Check if business software is running
            var businessSoftwareList = new List<string>();
            if (!string.IsNullOrWhiteSpace(businessSoftwareName))
            {
                businessSoftwareList = businessSoftwareName.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
            }
            
            if (businessSoftwareList.Count > 0 && BusinessSoftwareDetector.IsAnyRunning(businessSoftwareList))
            {
                string softwareList = string.Join(", ", businessSoftwareList);
                throw new InvalidOperationException(
                    $"Cannot start backup '{job.Name}': One or more business software applications ({softwareList}) are currently running. Please close them and try again.");
            }

            // Calculate total files and size before starting
            var (totalFiles, totalSize) = CalculateTotalFilesAndSize(job.SourcePath);
            context.UpdateProgress(0, totalFiles);
            
            // Initialize state
            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                State = JobState.ACTIVE,
                TotalFiles = totalFiles,
                TotalSize = totalSize,
                Progression = 0,
                NbFilesLeftToDo = totalFiles,
                NbFilesLeftToDoSize = totalSize,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });

            // Start the backup process with context
            int filesProcessed = 0;
            long bytesProcessed = 0;
            ProcessDirectory(job.SourcePath, job.TargetPath, job, ref filesProcessed, ref bytesProcessed, totalFiles, totalSize, progress, context);
            
            // Mark as completed
            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                State = JobState.END,
                TotalFiles = totalFiles,
                TotalSize = totalSize,
                Progression = 100,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
        }
        catch (OperationCanceledException)
        {
            // Job was stopped - log it
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                SourcePath = PathHelper.ToUncPath(job.SourcePath),
                TargetPath = PathHelper.ToUncPath(job.TargetPath),
                FileName = "",
                FileSize = 0,
                TransferTime = -1,
                ErrorMessage = "Backup stopped by user"
            });

            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                State = JobState.END,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalFiles = 0,
                TotalSize = 0,
                Progression = 0,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
            
            throw; // Re-throw so ParallelBackupCoordinator can handle it
        }
        catch (IOException ex)
        {
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                SourcePath = PathHelper.ToUncPath(job.SourcePath),
                TargetPath = PathHelper.ToUncPath(job.TargetPath),
                FileName = "",
                FileSize = 0,
                TransferTime = -1,
                ErrorMessage = ex.Message
            });

            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                State = JobState.END,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalFiles = 0,
                TotalSize = 0,
                Progression = 0,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
            
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                SourcePath = PathHelper.ToUncPath(job.SourcePath),
                TargetPath = PathHelper.ToUncPath(job.TargetPath),
                FileName = "",
                FileSize = 0,
                TransferTime = -1,
                ErrorMessage = ex.Message
            });

            _logger.UpdateState(new StateEntry
            {
                Name = job.Name,
                State = JobState.END,
                LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalFiles = 0,
                TotalSize = 0,
                Progression = 0,
                NbFilesLeftToDo = 0,
                NbFilesLeftToDoSize = 0,
                CurrentSourceFilePath = "",
                CurrentTargetFilePath = ""
            });
            
            throw;
        }
    }

    private bool IsSourceNewer(string sourceFile, string targetFile)
    {
        if (!File.Exists(targetFile))
        {
            return true;
        }

        var sourceLastWriteTime = File.GetLastWriteTime(sourceFile);
        var targetLastWriteTime = File.GetLastWriteTime(targetFile);

        return sourceLastWriteTime > targetLastWriteTime;
    }
    
    /// <summary>
    /// V3.0: Checks if a file has a priority extension.
    /// </summary>
    private bool IsPriorityFile(string filePath)
    {
        if (_priorityExtensions == null || _priorityExtensions.Count == 0)
        {
            return false;
        }
        
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        return _priorityExtensions.Any(ext => ext.ToLowerInvariant() == extension);
    }
}