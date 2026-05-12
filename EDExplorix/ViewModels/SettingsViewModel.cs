using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDExplorix.Services;

namespace EDExplorix.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ConfigurationService _config;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPathValid))]
    private string _journalPath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusIcon))]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    private bool _isStatusOk;
    
    public string StatusIcon => IsStatusOk ? "✓" : "✗";
    public string StatusColor => IsStatusOk ? "#40c040" : "#c04040";

    public bool IsPathValid => Directory.Exists(JournalPath) &&
                               Directory.GetFiles(JournalPath, "Journal.*.log").Any();

    public SettingsViewModel(ConfigurationService config)
    {
        _config = config;
        JournalPath = config.Config.JournalPath ?? string.Empty;
        ValidatePath();
    }

    [RelayCommand]
    private void Browse()
    {
        BrowseRequested?.Invoke();
    }

    public event Action? BrowseRequested;

    public void SetPath(string path)
    {
        JournalPath = path;
        ValidatePath();
    }

    private void ValidatePath()
    {
        if (string.IsNullOrEmpty(JournalPath))
        {
            StatusMessage = "No path set.";
            IsStatusOk = false;
            return;
        }

        if (!Directory.Exists(JournalPath))
        {
            StatusMessage = "Directory does not exist.";
            IsStatusOk = false;
            return;
        }

        var logs = Directory.GetFiles(JournalPath, "Journal.*.log");
        if (!logs.Any())
        {
            StatusMessage = "No journal files found in this directory.";
            IsStatusOk = false;
            return;
        }

        StatusMessage = $"OK – {logs.Length} journal files found.";
        IsStatusOk = true;
    }

    [RelayCommand]
    private void Save()
    {
        _config.Config.JournalPath = JournalPath;
        _config.Save();
        Saved?.Invoke();
    }

    public event Action? Saved;
}