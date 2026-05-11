using System.Text.Json.Serialization;

namespace EDExplorix.Models.Journal;

public class ScanEvent : JournalEvent
{
    [JsonPropertyName("ScanType")]
    string ScanType { get; set; } = string.Empty;

    [JsonPropertyName("BodyName")]
    public string BodyName { get; set; } = string.Empty;

    [JsonPropertyName("BodyID")]
    public int BodyId { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long SystemAddress { get; set; }

    [JsonPropertyName("StarType")]
    public string? StarType { get; set; }

    [JsonPropertyName("PlanetClass")]
    public string? PlanetClass { get; set; }

    [JsonPropertyName("MassEM")]
    public float? MassEM { get; set; }

    [JsonPropertyName("Radius")]
    public float? Radius { get; set; }

    [JsonPropertyName("SurfaceGravity")]
    public float? SurfaceGravity { get; set; }

    [JsonPropertyName("SurfaceTemperature")]
    public float? SurfaceTemperature { get; set; }

    [JsonPropertyName("SurfacePressure")]
    public float? SurfacePressure { get; set; }

    [JsonPropertyName("Landable")]
    public bool? Landable { get; set; }

    [JsonPropertyName("TerraformState")]
    public string? TerraformState { get; set; }

    [JsonPropertyName("Atmosphere")]
    public string? Atmosphere { get; set; }

    [JsonPropertyName("AtmosphereType")]
    public string? AtmosphereType { get; set; }

    [JsonPropertyName("Volcanism")]
    public string? Volcanism { get; set; }

    [JsonPropertyName("OrbitalPeriod")]
    public float? OrbitalPeriod { get; set; }

    [JsonPropertyName("RotationPeriod")]
    public float? RotationPeriod { get; set; }

    [JsonPropertyName("Eccentricity")]
    public float? Eccentricity { get; set; }

    [JsonPropertyName("WasDiscovered")]
    public bool WasDiscovered { get; set; }

    [JsonPropertyName("WasMapped")]
    public bool WasMapped { get; set; }
    
    public bool IsStar => StarType != null;
    public bool IsPlanet => PlanetClass != null;
}