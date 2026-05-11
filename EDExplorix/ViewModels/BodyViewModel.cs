using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDExplorix.Models.Database;

namespace EDExplorix.ViewModels;

public partial class BodyViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _showDetails;

    public string Name { get; }
    public string PlanetClass { get; }
    public float? MassEM { get; }
    public float? SurfaceGravity { get; }
    public float? SurfaceTemperature { get; }
    public bool? Landable { get; }
    public string? TerraformState { get; }
    public string? AtmosphereType { get; }
    public string? Volcanism { get; }
    public float? OrbitalPeriod { get; }
    public float? Eccentricity { get; }
    public bool WasDiscovered { get; }
    public bool WasMapped { get; }
    public bool IsValuable { get; }
    
    public long EstimatedValue { get; }
    public string ValueDisplay => EstimatedValue > 0 ? $"{EstimatedValue:N0} CR" : "N/A";
    public string AtmosphereDisplay => string.IsNullOrEmpty(AtmosphereType) ? "None" : AtmosphereType;
    public string VolcanismDisplay => string.IsNullOrEmpty(Volcanism) ? "None" : Volcanism;
    public string LandableDisplay => Landable.HasValue ? (Landable.Value ? "Yes" : "No") : "N/A";
    public string OrbitalPeriodDisplay => OrbitalPeriod.HasValue ? $"{OrbitalPeriod.Value / 86400f:F1} days" : "N/A";
    public string EccentricityDisplay => Eccentricity.HasValue ? $"{Eccentricity.Value:F3}" : "N/A";

    public string RowBackground => IsValuable ? "#1a2a0a" : "#0d1117";

    public string GravityDisplay => SurfaceGravity.HasValue
        ? $"{SurfaceGravity.Value / 9.81f:F2}g"
        : "N/A";

    public string TemperatureDisplay => SurfaceTemperature.HasValue
        ? $"{SurfaceTemperature.Value:F0} K"
        : "N/A";

    public string MassDisplay => MassEM.HasValue
        ? $"{MassEM.Value:F2} EM"
        : "N/A";

    public string TerraformDisplay => string.IsNullOrEmpty(TerraformState)
        ? "No"
        : TerraformState;

    public BodyViewModel(Body body)
    {
        Name = body.Name;
        PlanetClass = body.PlanetClass ?? "Unknown";
        MassEM = body.MassEM;
        SurfaceGravity = body.SurfaceGravity;
        SurfaceTemperature = body.SurfaceTemperature;
        Landable = body.Landable;
        TerraformState = body.TerraformState;
        AtmosphereType = body.AtmosphereType;
        Volcanism = body.Volcanism;
        OrbitalPeriod = body.OrbitalPeriod;
        Eccentricity = body.Eccentricity;
        WasDiscovered = body.WasDiscovered;
        WasMapped = body.WasMapped;
        EstimatedValue = body.EstimatedValue;
        IsValuable = body.IsValuable || EstimatedValue > 100000;
    }

    [RelayCommand]
    private void ToggleDetails() => ShowDetails = !ShowDetails;
}