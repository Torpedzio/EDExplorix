using System;
using System.IO;
using System.Text.Json;

namespace EDExplorix.Services;

public class AppConfig
{
    public string? JournalPath { get; set; }
}

public class ConfigurationService
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EDExplorix",
        "config.json"
    );

    public AppConfig Config { get; private set; } = new();

    public ConfigurationService()
    {
        Load();
    }

    private void Load()
    {
        if (!File.Exists(ConfigPath))
        {
            Config.JournalPath = DetectJournalPath();
            Save();
            return;
        }

        var json = File.ReadAllText(ConfigPath);
        Config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        var json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    public bool IsConfigured => !string.IsNullOrEmpty(Config.JournalPath)
                                && Directory.Exists(Config.JournalPath);

    private static string? DetectJournalPath()
    {
        // Windows
        var windows = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Saved Games", "Frontier Developments", "Elite Dangerous"
        );
        if (Directory.Exists(windows)) return windows;

        // Linux – Steam/Proton
        var linuxSteam = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".steam/steam/steamapps/compatdata/359320/pfx/drive_c/users/steamuser",
            "Saved Games/Frontier Developments/Elite Dangerous"
        );
        if (Directory.Exists(linuxSteam)) return linuxSteam;

        // Linux – Heroic/Proton
        var linuxHeroic = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Games/Heroic/Prefixes/default/Elite Dangerous/drive_c/users/steamuser",
            "Saved Games/Frontier Developments/Elite Dangerous"
        );
        if (Directory.Exists(linuxHeroic)) return linuxHeroic;

        return null;
    }
}