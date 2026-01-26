using System.Diagnostics;
using System.IO;
using EasySave.Shared;
using EasyLog;

namespace EasySave.Core.Services;

public class BackupService
{
    private readonly ILogger _logger;

    public BackupService(ILogger logger)
    {
        _logger = logger;
    }

    public void ExecuteBackup(BackupJob job)
    {
        try
        {
            // Start the backup process
            ProcessDirectory(job.SourcePath, job.TargetPath, job);
        }
        catch (Exception ex)
        {
            _logger.WriteLog(new LogEntry
            {
                JobName = job.Name,
                SourcePath = job.SourcePath,
                TargetPath = job.TargetPath,
                FileName = "",
                FileSize = 0,
                TransferTime = -1,
                ErrorMessage = ex.Message,
                Timestamp = DateTime.Now
            });
        }
    }

    private void ProcessDirectory(string sourceDir, string targetDir, BackupJob job)
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
                // Update state before copying
                _logger.UpdateState(new StateEntry
                {
                    Name = job.Name,
                    CurrentSourceFilePath = filePath,
                    CurrentTargetFilePath = targetFilePath,
                    State = JobState.ACTIVE,
                    LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
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

                    // Get file size
                    var fileInfo = new FileInfo(targetFilePath);

                    // Log the successful copy
                    _logger.WriteLog(new LogEntry
                    {
                        JobName = job.Name,
                        SourcePath = filePath,
                        TargetPath = targetFilePath,
                        FileName = fileName,
                        FileSize = fileInfo.Length,
                        TransferTime = stopwatch.ElapsedMilliseconds,
                        ErrorMessage = null, // Explicitly set null for nullable reference
                        Timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                // Get file size if possible
                long fileSize = 0;
                try
                {
                    if (File.Exists(filePath))
                    {
                        var fileInfo = new FileInfo(filePath);
                        fileSize = fileInfo.Length;
                    }
                }
                catch { }

                // Log the error
                _logger.WriteLog(new LogEntry
                {
                    JobName = job.Name,
                    SourcePath = filePath,
                    TargetPath = targetFilePath,
                    FileName = fileName,
                    FileSize = fileSize,
                    TransferTime = -1,
                    ErrorMessage = ex.Message,
                    Timestamp = DateTime.Now
                });
            }
        }

        // Recursively process subdirectories
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            string subDirName = Path.GetFileName(subDir);
            string targetSubDir = Path.Combine(targetDir, subDirName);
            ProcessDirectory(subDir, targetSubDir, job);
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