using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySave.ConsoleApp.Config;
using EasyLog;
using EasySave.GUI.Helpers;
using EasySave.ConsoleApp.Languages;

namespace EasySave.GUI.ViewModels;

/// <summary>
/// ViewModel for application settings.
/// </summary>
public partial class SettingsViewModel : ViewModelBase
{
    public LocalizationManager Localization => LocalizationManager.Instance;
    [ObservableProperty]
    private ObservableCollection<string> _extensionsToEncrypt = new();

    [ObservableProperty]
    private string _newExtension = string.Empty;

    [ObservableProperty]
    private string? _selectedExtension;

    [ObservableProperty]
    private ObservableCollection<string> _businessSoftwareNames = new();

    [ObservableProperty]
    private string _newBusinessSoftware = string.Empty;

    [ObservableProperty]
    private string? _selectedBusinessSoftware;

    [ObservableProperty]
    private ObservableCollection<string> _priorityExtensions = new();

    [ObservableProperty]
    private string _newPriorityExtension = string.Empty;

    [ObservableProperty]
    private string? _selectedPriorityExtension;

    [ObservableProperty]
    private string _cryptoSoftPath = string.Empty;

    [ObservableProperty]
    private long _maxLargeFileSize = 0;

    [ObservableProperty]
    private LogFormat _selectedLogFormat = LogFormat.JSON;

    // V3.0 Docker/Azure Centralized Logging
    [ObservableProperty]
    private bool _enableNetworkLogging = false;

    [ObservableProperty]
    private string _logServerIp = "127.0.0.1";

    [ObservableProperty]
    private int _logServerPort = 9000;

    public ObservableCollection<LogFormat> LogFormats { get; }
    
    public bool DialogResult { get; private set; }
    
    public event EventHandler? CloseRequested;

    public SettingsViewModel()
    {
        LogFormats = new ObservableCollection<LogFormat>(Enum.GetValues<LogFormat>());
        LoadSettings();
    }

    [RelayCommand]
    private void AddExtension()
    {
        if (!string.IsNullOrWhiteSpace(NewExtension))
        {
            var ext = NewExtension.Trim();
            if (!ext.StartsWith("."))
                ext = "." + ext;

            if (!ExtensionsToEncrypt.Contains(ext))
            {
                ExtensionsToEncrypt.Add(ext);
                NewExtension = string.Empty;
            }
        }
    }

    [RelayCommand]
    private void RemoveExtension()
    {
        if (SelectedExtension != null)
        {
            ExtensionsToEncrypt.Remove(SelectedExtension);
            SelectedExtension = null;
        }
    }

    [RelayCommand]
    private void AddBusinessSoftware()
    {
        if (!string.IsNullOrWhiteSpace(NewBusinessSoftware))
        {
            var software = NewBusinessSoftware.Trim();
            // Remove .exe if provided
            if (software.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                software = software[..^4];

            if (!BusinessSoftwareNames.Contains(software))
            {
                BusinessSoftwareNames.Add(software);
                NewBusinessSoftware = string.Empty;
            }
        }
    }

    [RelayCommand]
    private void RemoveBusinessSoftware()
    {
        if (SelectedBusinessSoftware != null)
        {
            BusinessSoftwareNames.Remove(SelectedBusinessSoftware);
            SelectedBusinessSoftware = null;
        }
    }

    [RelayCommand]
    private void AddPriorityExtension()
    {
        if (!string.IsNullOrWhiteSpace(NewPriorityExtension))
        {
            var ext = NewPriorityExtension.Trim();
            if (!ext.StartsWith("."))
                ext = "." + ext;

            if (!PriorityExtensions.Contains(ext))
            {
                PriorityExtensions.Add(ext);
                NewPriorityExtension = string.Empty;
            }
        }
    }

    [RelayCommand]
    private void RemovePriorityExtension()
    {
        if (SelectedPriorityExtension != null)
        {
            PriorityExtensions.Remove(SelectedPriorityExtension);
            SelectedPriorityExtension = null;
        }
    }

    [RelayCommand]
    private async Task BrowseCryptoSoft()
    {
        var dialog = new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = LanguageManager.Instance.GetString("Browse_CryptoSoft_Title"),
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new Avalonia.Platform.Storage.FilePickerFileType("Executable Files")
                {
                    Patterns = new[] { "*.exe" }
                }
            }
        };

        // Get the SettingsWindow instead of MainWindow
        var window = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.Windows.FirstOrDefault(w => w is Views.SettingsWindow)
            : null;

        if (window != null)
        {
            var files = await window.StorageProvider.OpenFilePickerAsync(dialog);
            if (files.Count > 0)
            {
                CryptoSoftPath = files[0].Path.LocalPath;
            }
            
            // Bring Settings window back to focus
            window.Activate();
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveSettings();
        DialogResult = true;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void LoadSettings()
    {
        ExtensionsToEncrypt.Clear();
        foreach (var ext in AppConfig.GetExtensionsToEncrypt())
        {
            ExtensionsToEncrypt.Add(ext);
        }

        BusinessSoftwareNames.Clear();
        foreach (var software in AppConfig.GetBusinessSoftwareNames())
        {
            BusinessSoftwareNames.Add(software);
        }

        PriorityExtensions.Clear();
        foreach (var ext in AppConfig.GetPriorityExtensions())
        {
            PriorityExtensions.Add(ext);
        }

        CryptoSoftPath = AppConfig.GetCryptoSoftPath();
        MaxLargeFileSize = AppConfig.GetMaxLargeFileSize();
        SelectedLogFormat = AppConfig.GetLogFormat();

        // V3.0: Docker / Azure Centralized Logging
        EnableNetworkLogging = AppConfig.GetEnableNetworkLogging();
        LogServerIp = AppConfig.GetLogServerIp();
        LogServerPort = AppConfig.GetLogServerPort();
    }

    private void SaveSettings()
    {
        AppConfig.SetExtensionsToEncrypt(ExtensionsToEncrypt.ToList());
        AppConfig.SetBusinessSoftwareNames(BusinessSoftwareNames.ToList());
        AppConfig.SetPriorityExtensions(PriorityExtensions.ToList());
        AppConfig.SetCryptoSoftPath(CryptoSoftPath);
        AppConfig.SetMaxLargeFileSize(MaxLargeFileSize);
        AppConfig.SetLogFormat(SelectedLogFormat);

        // V3.0: Docker / Azure Centralized Logging
        AppConfig.SetEnableNetworkLogging(EnableNetworkLogging);
        AppConfig.SetLogServerIp(LogServerIp);
        AppConfig.SetLogServerPort(LogServerPort);
    }
}
