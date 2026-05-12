using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDExplorix.Models;
using EDExplorix.Models.Journal;
using EDExplorix.Services;
using EDExplorix.Services.Database;
using EDExplorix.Services.Journal;
using EDExplorix.Views;

namespace EDExplorix.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ExplorationService _explorationService;
    private readonly JournalWatcher _watcher;

    public ObservableCollection<StarSystemViewModel> Systems { get; } = [];

    private readonly ConfigurationService _config = new();
    
    private readonly Dictionary<(long SystemAddress, int BodyId), (int Bio, int Geo)> _pendingSignals = new();
    private SortOption _selectedSort = SortOption.ScanOrder;

    public MainWindowViewModel()
    {
        Console.WriteLine($"Journal path: {_config.Config.JournalPath}");
        Console.WriteLine($"IsConfigured: {_config.IsConfigured}");
        var db = new AppDbContext();
        _watcher = new JournalWatcher();
        _explorationService = new ExplorationService(db, _watcher);

        _watcher.Events.Subscribe(OnJournalEvent);

        if (_config.IsConfigured)
            _watcher.Start(_config.Config.JournalPath!);
        
        //Tymczasowe - tylko do testów UI
        //Task.Delay(500).ContinueWith(_ => SimulateData());
    }

    private void OnJournalEvent(JournalEvent journalEvent)
    {
        Console.WriteLine($"OnJournalEvent: {journalEvent.Event}");
        switch (journalEvent)
        {
            case FSDJumpEvent jump:
                HandleFSDJump(jump);
                break;
            case ScanEvent scan:
                Console.WriteLine($"Scan: {scan.BodyName} IsPlanet={scan.IsPlanet} IsStar={scan.IsStar}");
                HandleScan(scan);
                break;
            case FSSBodySignalsEvent signals:
                HandleBodySignals(signals);
                break;
            case FSSDiscoveryScanEvent fss:
                HandleFSSDiscoveryScan(fss);
                break;
            case SAAScanCompleteEvent saa:
                HandleSAAScanComplete(saa);
                break;
        }
    }

    private void HandleFSDJump(FSDJumpEvent jump)
    {
        Console.WriteLine($"HandleFSDJump: {jump.StarSystem} {jump.SystemAddress}");
        foreach (var s in Systems)
            s.IsExpanded = false;

        var existing = Systems.FirstOrDefault(s => s.SystemAddress == jump.SystemAddress);
        if (existing != null)
        {
            existing.IsExpanded = true;
            return;
        }

        var vm = new StarSystemViewModel(new Models.Database.StarSystem
        {
            SystemAddress = jump.SystemAddress,
            Name = jump.StarSystem,
            X = jump.StarPos?.Length > 0 ? jump.StarPos[0] : 0,
            Y = jump.StarPos?.Length > 1 ? jump.StarPos[1] : 0,
            Z = jump.StarPos?.Length > 2 ? jump.StarPos[2] : 0,
            FirstVisited = jump.Timestamp
        }) { IsExpanded = true };

        Avalonia.Threading.Dispatcher.UIThread.Post(() => Systems.Insert(0, vm));
    }

    private void HandleScan(ScanEvent scan)
    {
        var systemVm = Systems.FirstOrDefault(s => s.SystemAddress == scan.SystemAddress);
        Console.WriteLine($"HandleScan: {scan.BodyName}, systemVm={systemVm != null}, Systems.Count={Systems.Count}");
        if (systemVm == null)
            return;

        var body = new Models.Database.Body
        {
            BodyId = scan.BodyId,
            SystemAddress = scan.SystemAddress,
            Name = scan.BodyName,
            PlanetClass = scan.PlanetClass,
            MassEM = scan.MassEM,
            Radius = scan.Radius,
            SurfaceGravity = scan.SurfaceGravity,
            SurfaceTemperature = scan.SurfaceTemperature,
            SurfacePressure = scan.SurfacePressure,
            Landable = scan.Landable,
            TerraformState = scan.TerraformState,
            AtmosphereType = scan.AtmosphereType,
            Volcanism = scan.Volcanism,
            OrbitalPeriod = scan.OrbitalPeriod,
            Eccentricity = scan.Eccentricity,
            WasDiscovered = scan.WasDiscovered,
            WasMapped = scan.WasMapped,
            ScannedAt = scan.Timestamp
        };

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            systemVm.AddBody(body);

            if (_pendingSignals.TryGetValue((scan.SystemAddress, scan.BodyId), out var signals))
            {
                var bodyVm = systemVm.Bodies.FirstOrDefault(b => b.BodyId == scan.BodyId);
                if (bodyVm != null)
                {
                    bodyVm.UpdateSignals(signals.Bio, signals.Geo);
                }
                _pendingSignals.Remove((scan.SystemAddress, scan.BodyId));
            }
        });
    }
    
    private void HandleBodySignals(FSSBodySignalsEvent signals)
    {
        var bio = signals.Signals
            .FirstOrDefault(s => s.TypeDisplay == "Biological")?.Count ?? 0;
        var geo = signals.Signals
            .FirstOrDefault(s => s.TypeDisplay == "Geological")?.Count ?? 0;

        _pendingSignals[(signals.SystemAddress, signals.BodyId)] = (bio, geo);
        ApplySignals(signals.SystemAddress, signals.BodyId, bio, geo);
    }
    
    private void ApplySignals(long systemAddress, int bodyId, int bio, int geo)
    {
        var systemVm = Systems.FirstOrDefault(s => s.SystemAddress == systemAddress);
        var bodyVm = systemVm?.Bodies.FirstOrDefault(b => b.BodyId == bodyId);
        if (bodyVm == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            bodyVm.UpdateSignals(bio, geo));
    }
    
    private void HandleFSSDiscoveryScan(FSSDiscoveryScanEvent fss)
    {
        var systemVm = Systems.FirstOrDefault(s => s.SystemAddress == fss.SystemAddress);
        if (systemVm == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            systemVm.BodyCount = fss.BodyCount);
    }
    
    private void HandleSAAScanComplete(SAAScanCompleteEvent saa)
    {
        var systemVm = Systems.FirstOrDefault(s => s.SystemAddress == saa.SystemAddress);
        var bodyVm = systemVm?.Bodies.FirstOrDefault(b => b.BodyId == saa.BodyId);
        if (bodyVm == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() => bodyVm.SetMapped());
    }
    
    public SortOption SelectedSort
    {
        get => _selectedSort;
        set
        {
            _selectedSort = value;
            OnPropertyChanged(nameof(SelectedSort));
            foreach (var system in Systems)
                system.ApplySort(value);
        }
    }
    public List<SortOption> SortOptions => Enum.GetValues<SortOption>().ToList();
    
    [RelayCommand]
    private void OpenSettings()
    {
        var vm = new SettingsViewModel(_config);
        var window = new SettingsWindow(vm);
        vm.Saved += () =>
        {
            if (_config.IsConfigured)
                _watcher.Start(_config.Config.JournalPath!);
        };
        window.ShowDialog(App.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null);
    }
    
    
    // Tymczasowe - tylko do testów UI
    public void SimulateData()
    {
        HandleFSDJump(new FSDJumpEvent
        {
            StarSystem = "Shinrarta Dezhra",
            SystemAddress = 111111,
            StarPos = [55.71875f, 17.59375f, 27.15625f],
            Timestamp = DateTime.UtcNow
        });
        
        var systemVm = Systems.FirstOrDefault(s => s.SystemAddress == 111111);
        if (systemVm != null)
            systemVm.BodyCount = 3;

        HandleScan(new ScanEvent
        {
            BodyName = "Shinrarta Dezhra 1",
            BodyId = 1,
            SystemAddress = 111111,
            PlanetClass = "Earthlike body",
            MassEM = 0.95f,
            SurfaceGravity = 9.5f,
            SurfaceTemperature = 290f,
            Landable = false,
            TerraformState = "",
            AtmosphereType = "Nitrogen",
            WasDiscovered = false,
            WasMapped = false,
            Timestamp = DateTime.UtcNow
        });

        HandleScan(new ScanEvent
        {
            BodyName = "Shinrarta Dezhra 2",
            BodyId = 2,
            SystemAddress = 111111,
            PlanetClass = "Water world",
            MassEM = 1.2f,
            SurfaceGravity = 11.2f,
            SurfaceTemperature = 310f,
            Landable = false,
            TerraformState = "Terraformable",
            AtmosphereType = "Water",
            WasDiscovered = true,
            WasMapped = false,
            Timestamp = DateTime.UtcNow
        });

        HandleScan(new ScanEvent
        {
            BodyName = "Shinrarta Dezhra 3",
            BodyId = 3,
            SystemAddress = 111111,
            PlanetClass = "Rocky body",
            MassEM = 0.1f,
            SurfaceGravity = 2.1f,
            SurfaceTemperature = 180f,
            Landable = true,
            TerraformState = "",
            AtmosphereType = "",
            WasDiscovered = false,
            WasMapped = false,
            Timestamp = DateTime.UtcNow
        });
        
        
    }
}