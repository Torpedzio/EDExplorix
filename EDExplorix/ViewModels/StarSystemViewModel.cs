using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDExplorix.Models.Database;

namespace EDExplorix.ViewModels;

public partial class StarSystemViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BodyCountDisplay))]
    private int _bodyCount;

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
        Bodies.Insert(0, new BodyViewModel(body));
        OnPropertyChanged(nameof(BodyCountDisplay));
    }
}