using System.Text.Json;
using EasySave.Shared;

namespace EasySave.Core.Services;

/// <summary>
/// Manages backup jobs: loading, saving, adding, deleting, and retrieving jobs.
/// Maximum of 5 jobs allowed as per specifications.
/// </summary>
public class JobManager
{
    private static readonly string JobsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ProSoft",
        "EasySave",
        "jobs.json"
    );
    private const int MaxJobs = 5;
    public static List<BackupJob> Jobs { get; private set; } = new();

    // Load jobs from the JSON file
    public static void LoadJobs()
    {
        if (File.Exists(JobsFilePath))
        {
            string json = File.ReadAllText(JobsFilePath);
            Jobs = JsonSerializer.Deserialize<List<BackupJob>>(json) ?? new List<BackupJob>();
        }
        else
        {
            Jobs = new List<BackupJob>();
        }
    }

    /// <summary>
    /// Saves all jobs to the JSON configuration file.
    /// Creates the directory if it doesn't exist.
    /// </summary>
    public static void SaveJobs()
    {
        string? directory = Path.GetDirectoryName(JobsFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        string json = JsonSerializer.Serialize(Jobs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(JobsFilePath, json);
    }

    // Add a new job
    public static void AddJob(string name, string source, string target, BackupType type)
    {
        if (Jobs.Count >= MaxJobs)
        {
            throw new InvalidOperationException("Cannot add more than 5 jobs.");
        }

        Jobs.Add(new BackupJob(name, source, target, type));
        SaveJobs();
    }

    // Delete a job by index
    public static void DeleteJob(int index)
    {
        if (index < 0 || index >= Jobs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid job index.");
        }

        Jobs.RemoveAt(index);
        SaveJobs();
    }

    // Get a job by index
    public static BackupJob GetJob(int index)
    {
        if (index < 0 || index >= Jobs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid job index.");
        }

        return Jobs[index];
    }
}