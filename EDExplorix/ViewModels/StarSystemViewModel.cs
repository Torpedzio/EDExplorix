using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDExplorix.Models;
using EDExplorix.Models.Database;

namespace EDExplorix.ViewModels;

public partial class StarSystemViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BodyCountDisplay))]
    private int _bodyCount;
    private SortOption _currentSort = SortOption.ScanOrder;
    private readonly List<BodyViewModel> _allBodies = [];
    public long SystemAddress { get; }
    public string Name { get; }
    public DateTime FirstVisited { get; }
    public ObservableCollection<BodyViewModel> Bodies { get; } = [];
    
    public string BodyCountDisplay => BodyCount > 0 
        ? $"{Bodies.Count}/{BodyCount} bodies" 
        : $"{Bodies.Count} bodies";
    public string ExpandIcon => IsExpanded ? "▲" : "▼";
    
    [RelayCommand]
    private void ToggleExpanded() => IsExpanded = !IsExpanded;

    public StarSystemViewModel(StarSystem system)
    {
        SystemAddress = system.SystemAddress;
        Name = system.Name;
        FirstVisited = system.FirstVisited;
        BodyCount = system.BodyCount;
    }

    public void AddBody(Body body)
    {
        _allBodies.Add(new BodyViewModel(body));
        ApplySort();
        OnPropertyChanged(nameof(BodyCountDisplay));
    }
    
    public void ApplySort(SortOption? sort = null)
    {
        if (sort.HasValue)
            _currentSort = sort.Value;

        var sorted = _currentSort switch
        {
            SortOption.ValueDescending => _allBodies.OrderByDescending(b => b.EstimatedValue),
            SortOption.ValueAscending => _allBodies.OrderBy(b => b.EstimatedValue),
            SortOption.MassDescending => _allBodies.OrderByDescending(b => b.MassEM ?? 0),
            SortOption.MassAscending => _allBodies.OrderBy(b => b.MassEM ?? 0),
            SortOption.TemperatureDescending => _allBodies.OrderByDescending(b => b.SurfaceTemperature ?? 0),
            SortOption.TemperatureAscending => _allBodies.OrderBy(b => b.SurfaceTemperature ?? 0),
            SortOption.NameAscending => _allBodies.OrderBy(b => b.Name),
            _ => _allBodies.AsEnumerable()
        };

        Bodies.Clear();
        foreach (var body in sorted)
            Bodies.Add(body);
    }
}