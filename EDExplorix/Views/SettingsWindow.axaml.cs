using Avalonia.Controls;
using Avalonia.Platform.Storage;
using EDExplorix.ViewModels;

namespace EDExplorix.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        vm.BrowseRequested += async () =>
        {
            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Elite Dangerous journal folder",
                AllowMultiple = false
            });

            if (folders.Count > 0)
                vm.SetPath(folders[0].Path.LocalPath);
        };

        vm.Saved += Close;
    }
}