using System.Text.Json;
using EasySave.Shared;

namespace EasySave.Core.Services;

public class JobManager
{
    private const string JobsFilePath = "C:/EasySave/jobs.json";
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

    // Save jobs to the JSON file
    public static void SaveJobs()
    {
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