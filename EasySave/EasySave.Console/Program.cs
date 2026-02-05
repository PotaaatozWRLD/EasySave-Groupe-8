using EasySave.Shared;
using EasySave.Core.Services;
using EasyLog;
using EasySave.ConsoleApp.Languages;
using EasySave.ConsoleApp.Config;
using System.IO;

namespace EasySave.ConsoleApp;

class Program
{
    private static LanguageManager _lang = LanguageManager.Instance;

    static void Main(string[] args)
    {
        // Use AppData for configuration files to respect server deployment standards
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProSoft",
            "EasySave"
        );
        string logDirectory = Path.Combine(appDataPath, "Logs");
        string stateFilePath = Path.Combine(appDataPath, "state.json");

        ILogger logger = new JsonLogger(logDirectory, stateFilePath);
        BackupService backupService = new BackupService(logger);

        JobManager.LoadJobs();

        // Show language selection menu at every startup
        ShowLanguageSelectionMenu();

        if (args.Length == 0)
        {
            // MODE 1: Console Menu
            while (true)
            {
                Console.Clear();
                Console.WriteLine("============================");
                Console.WriteLine($"      {_lang.GetString("Welcome")}       ");
                Console.WriteLine("============================");
                Console.WriteLine($"1. {_lang.GetString("Menu_List")}");
                Console.WriteLine($"2. {_lang.GetString("Menu_Create")}");
                Console.WriteLine($"3. {_lang.GetString("Menu_Edit")}");
                Console.WriteLine($"4. {_lang.GetString("Menu_Delete")}");
                Console.WriteLine($"5. {_lang.GetString("Menu_Run")}");
                Console.WriteLine($"6. {_lang.GetString("Menu_RunAll")}");
                Console.WriteLine($"7. {_lang.GetString("Menu_Lang")}");
                Console.WriteLine($"8. {_lang.GetString("Menu_Exit")}");
                Console.Write(_lang.GetString("Msg_SelectOption"));

                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        ListJobs();
                        break;
                    case "2":
                        CreateJob();
                        break;
                    case "3":
                        EditJob();
                        break;
                    case "4":
                        DeleteJob();
                        break;
                    case "5":
                        RunJob(backupService);
                        break;
                    case "6":
                        RunAllJobs(backupService);
                        break;
                    case "7":
                        ChangeLanguage();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine(_lang.GetString("Error_InvalidOption"));
                        Console.ReadKey();
                        break;
                }
            }
        }
        else
        {
            // MODE 2: Run jobs based on arguments
            var jobIndexes = ParseJobIndexes(args[0]);
            foreach (var index in jobIndexes)
            {
                try
                {
                    var job = JobManager.GetJob(index - 1);
                    backupService.ExecuteBackup(job);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine(string.Format(_lang.GetString("Error_JobNotFound"), index));
                }
            }
        }
    }

    static void ChangeLanguage()
    {
        _lang.ToggleLanguage();
        Console.Clear();
        Console.WriteLine(_lang.GetString("Msg_LangChanged"));
        System.Threading.Thread.Sleep(1000);
    }

    static void ListJobs()
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine($"        {_lang.GetString("JobsList_Title")}           ");
        Console.WriteLine("============================");

        if (JobManager.Jobs.Count == 0)
        {
            Console.WriteLine(_lang.GetString("JobsList_NoJobs"));
        }
        else
        {
            for (int i = 0; i < JobManager.Jobs.Count; i++)
            {
                var job = JobManager.Jobs[i];
                Console.WriteLine(string.Format(_lang.GetString("JobsList_Format"), 
                    i + 1, job.Name, job.SourcePath, job.TargetPath, job.Type));
            }
        }

        Console.WriteLine(_lang.GetString("Msg_PressKey"));
        Console.ReadKey();
    }

    static void CreateJob()
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine($"       {_lang.GetString("CreateJob_Title")}         ");
        Console.WriteLine("============================");

        Console.Write(_lang.GetString("CreateJob_EnterName"));
        string name = Console.ReadLine() ?? "";

        Console.Write(_lang.GetString("CreateJob_EnterSource"));
        string source = Console.ReadLine() ?? "";

        Console.Write(_lang.GetString("CreateJob_EnterTarget"));
        string target = Console.ReadLine() ?? "";

        Console.WriteLine();
        Console.WriteLine("1. Full Backup");
        Console.WriteLine("2. Differential Backup");
        Console.Write(_lang.GetString("CreateJob_EnterType"));
        string typeInput = Console.ReadLine() ?? "";
        
        BackupType type = BackupType.Full; // Default
        if (int.TryParse(typeInput, out int typeChoice))
        {
            type = typeChoice == 2 ? BackupType.Differential : BackupType.Full;
        }
        else if (typeInput.Equals("Full", StringComparison.OrdinalIgnoreCase))
        {
            type = BackupType.Full;
        }
        else if (typeInput.Equals("Differential", StringComparison.OrdinalIgnoreCase))
        {
            type = BackupType.Differential;
        }

        try
        {
            JobManager.AddJob(name, source, target, type);
            Console.WriteLine(_lang.GetString("CreateJob_Success"));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.ReadKey();
    }

    static void EditJob()
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine($"       {_lang.GetString("EditJob_Title")}         ");
        Console.WriteLine("============================");

        if (JobManager.Jobs.Count == 0)
        {
            Console.WriteLine(_lang.GetString("JobsList_NoJobs"));
            Console.WriteLine(_lang.GetString("Msg_PressKey"));
            Console.ReadKey();
            return;
        }

        // Liste les jobs disponibles
        Console.WriteLine(_lang.GetString("EditJob_AvailableJobs"));
        for (int i = 0; i < JobManager.Jobs.Count; i++)
        {
            var job = JobManager.Jobs[i];
            Console.WriteLine(string.Format(_lang.GetString("JobsList_Format"), 
                i + 1, job.Name, job.SourcePath, job.TargetPath, job.Type));
        }
        Console.WriteLine();

        Console.Write(_lang.GetString("EditJob_EnterIndex"));
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > JobManager.Jobs.Count)
        {
            Console.WriteLine(_lang.GetString("Error_InvalidJobIndex"));
            Console.ReadKey();
            return;
        }

        var jobToEdit = JobManager.Jobs[index - 1];
        Console.WriteLine();
        Console.WriteLine(_lang.GetString("EditJob_CurrentValues"));
        Console.WriteLine(string.Format(_lang.GetString("JobsList_Format"), 
            index, jobToEdit.Name, jobToEdit.SourcePath, jobToEdit.TargetPath, jobToEdit.Type));
        Console.WriteLine();

        // Nom
        Console.Write(_lang.GetString("EditJob_NewName"));
        string newName = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(newName))
            jobToEdit.Name = newName;

        // Source
        Console.Write(_lang.GetString("EditJob_NewSource"));
        string newSource = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(newSource))
            jobToEdit.SourcePath = newSource;

        // Target
        Console.Write(_lang.GetString("EditJob_NewTarget"));
        string newTarget = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(newTarget))
            jobToEdit.TargetPath = newTarget;

        // Type
        Console.WriteLine();
        Console.WriteLine("1. Full Backup");
        Console.WriteLine("2. Differential Backup");
        Console.Write(_lang.GetString("EditJob_NewType"));
        string typeInput = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(typeInput))
        {
            if (int.TryParse(typeInput, out int typeChoice))
            {
                jobToEdit.Type = typeChoice == 2 ? BackupType.Differential : BackupType.Full;
            }
            else if (typeInput.Equals("Full", StringComparison.OrdinalIgnoreCase))
            {
                jobToEdit.Type = BackupType.Full;
            }
            else if (typeInput.Equals("Differential", StringComparison.OrdinalIgnoreCase))
            {
                jobToEdit.Type = BackupType.Differential;
            }
        }

        JobManager.SaveJobs();
        Console.WriteLine(_lang.GetString("EditJob_Success"));
        Console.ReadKey();
    }

    static void DeleteJob()
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine($"       {_lang.GetString("DeleteJob_Title")}         ");
        Console.WriteLine("============================");

        Console.Write(_lang.GetString("DeleteJob_EnterIndex"));
        if (int.TryParse(Console.ReadLine(), out int index))
        {
            try
            {
                JobManager.DeleteJob(index - 1);
                Console.WriteLine(_lang.GetString("DeleteJob_Success"));
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine(_lang.GetString("Error_InvalidJobIndex"));
            }
        }
        else
        {
            Console.WriteLine(_lang.GetString("Error_Input"));
        }

        Console.ReadKey();
    }

    static void RunAllJobs(BackupService backupService)
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine("   RUN ALL JOBS            ");
        Console.WriteLine("============================");

        if (JobManager.Jobs.Count == 0)
        {
            Console.WriteLine("No jobs available.");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Found {JobManager.Jobs.Count} job(s). Starting sequential execution...\n");

        for (int i = 0; i < JobManager.Jobs.Count; i++)
        {
            var job = JobManager.Jobs[i];
            Console.WriteLine($"[{i + 1}/{JobManager.Jobs.Count}] Executing job: {job.Name}...");
            
            try
            {
                backupService.ExecuteBackup(job);
                Console.WriteLine($"✓ Job '{job.Name}' completed successfully.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Job '{job.Name}' failed: {ex.Message}\n");
            }
        }

        Console.WriteLine("All jobs execution completed. Press any key to return to the menu...");
        Console.ReadKey();
    }

    static void RunJob(BackupService backupService)
    {
        Console.Clear();
        Console.WriteLine("============================");
        Console.WriteLine($"        {_lang.GetString("RunJob_Title")}           ");
        Console.WriteLine("============================");
        Console.WriteLine("Examples: 1 (single) | 1-3 (range) | 1;3 (list)");
        Console.WriteLine();

        Console.Write(_lang.GetString("RunJob_EnterIndex"));
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(_lang.GetString("Error_Input"));
            Console.ReadKey();
            return;
        }

        try
        {
            var jobIndexes = ParseJobIndexes(input);
            int successCount = 0;
            int failCount = 0;

            foreach (var index in jobIndexes)
            {
                try
                {
                    var job = JobManager.GetJob(index - 1);
                    Console.WriteLine($"\nExecuting job {index}: {job.Name}...");
                    backupService.ExecuteBackup(job);
                    Console.WriteLine($"✓ Job {index} completed successfully.");
                    successCount++;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine($"✗ Job {index} not found.");
                    failCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Job {index} failed: {ex.Message}");
                    failCount++;
                }
            }

            Console.WriteLine($"\nSummary: {successCount} succeeded, {failCount} failed.");
        }
        catch (Exception)
        {
            Console.WriteLine(_lang.GetString("Error_Input"));
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    static IEnumerable<int> ParseJobIndexes(string input)
    {
        var indexes = new List<int>();
        foreach (var part in input.Split(';'))
        {
            if (part.Contains('-'))
            {
                var range = part.Split('-').Select(int.Parse).ToArray();
                indexes.AddRange(Enumerable.Range(range[0], range[1] - range[0] + 1));
            }
            else if (int.TryParse(part, out int singleIndex))
            {
                indexes.Add(singleIndex);
            }
        }
        return indexes;
    }

    static void ShowLanguageSelectionMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("============================");
            Console.WriteLine("   LANGUAGE SELECTION      ");
            Console.WriteLine("   SÉLECTION DE LANGUE     ");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.WriteLine();
            Console.Write("Select / Choisir (1-2): ");

            string? input = Console.ReadLine();
            
            if (input == "1")
            {
                LanguageManager.Instance.ChangeLanguage("en");
                Console.WriteLine("\nLanguage set to English. Press any key to continue...");
                Console.ReadKey();
                break;
            }
            else if (input == "2")
            {
                LanguageManager.Instance.ChangeLanguage("fr");
                Console.WriteLine("\nLangue définie en Français. Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                break;
            }
            else
            {
                Console.WriteLine("\nInvalid option / Option invalide. Press any key to try again...");
                Console.ReadKey();
            }
        }
    }
}
