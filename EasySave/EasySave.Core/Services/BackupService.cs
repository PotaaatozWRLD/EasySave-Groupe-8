using System.Diagnostics;
using System.IO;
using EasySave.Shared;
using EasyLog;
using EasySave.Core.Helpers;

namespace EasySave.Core.Services;

/// <summary>
/// Service responsible for executing backup jobs.
/// Handles file copying, progress tracking, and logging.
/// </summary>
public class BackupService
{
    private readonly ILogger _logger;

    public BackupService(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes a backup job with full progress tracking.
    /// v2.0: Prevents backup if business software is running.
    /// </summary>
    /// <param name="job">The backup job to execute</param>
    /// <param name="businessSoftwareName">Optional name of business software to check (v2.0). If running, backup will be blocked.</param>
    /// <exception cref="InvalidOperationException">Thrown when business software is running and backup cannot proceed.</exception>
    public void ExecuteBackup(BackupJob job, string? businessSoftwareName = null)
    {
        try
        {
            // v2.0: Check if business software is running
            if (!string.IsNullOrWhiteSpace(businessSoftwareName) && 
                BusinessSoftwareDetector.IsRunning(businessSoftwareName))
            {
                throw new InvalidOperationException(
                    $"Cannot start backup '{job.Name}': Business software '{businessSoftwareName}' is currently running. Please close it and try again.");
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
            ProcessDirectory(job.SourcePath, job.TargetPath, job, ref filesProcessed, ref bytesProcessed, totalFiles, totalSize);
            
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
    /// </summary>
    private void ProcessDirectory(string sourceDir, string targetDir, BackupJob job, 
                                  ref int filesProcessed, ref long bytesProcessed, 
                                  int totalFiles, long totalSize)
    {
        // Ensure the target directory exists
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        // Process all files in the directory
        foreach (var filePath in Directory.GetFiles(sourceDir))
        {
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

                    using (var sourceStream = File.OpenRead(filePath))
                    using (var targetStream = File.Create(targetFilePath))
                    {
                        sourceStream.CopyTo(targetStream);
                    }

                    stopwatch.Stop();

                    // Log the successful copy
                    _logger.WriteLog(new LogEntry
                    {
                        JobName = job.Name,
                        SourcePath = PathHelper.ToUncPath(filePath),
                        TargetPath = PathHelper.ToUncPath(targetFilePath),
                        FileName = fileName,
                        FileSize = fileSize,
                        TransferTime = stopwatch.ElapsedMilliseconds,
                        ErrorMessage = null,
                        Timestamp = DateTime.Now
                    });
                }

                // Update progress counters
                filesProcessed++;
                bytesProcessed += fileSize;
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
            ProcessDirectory(subDir, targetSubDir, job, ref filesProcessed, ref bytesProcessed, totalFiles, totalSize);
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
}