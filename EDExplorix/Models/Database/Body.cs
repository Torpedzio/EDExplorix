using System;

namespace EDExplorix.Models.Database;

public class Body
{
    public int Id { get; set; }
    public int BodyId { get; set; }
    public long SystemAddress { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? StarType { get; set; }
    public string? PlanetClass { get; set; }
    public float? MassEM { get; set; }
    public float? Radius { get; set; }
    public float? SurfaceGravity { get; set; }
    public float? SurfaceTemperature { get; set; }
    public float? SurfacePressure { get; set; }
    public bool? Landable { get; set; }
    public string? TerraformState { get; set; }
    public string? AtmosphereType { get; set; }
    public string? Volcanism { get; set; }
    public float? OrbitalPeriod { get; set; }
    public float? Eccentricity { get; set; }
    public bool WasDiscovered { get; set; }
    public bool WasMapped { get; set; }
    public DateTime ScannedAt { get; set; }
    public StarSystem StarSystem { get; set; } = null!;
    public int BiologicalSignals { get; set; }
    public int GeologicalSignals { get; set; }

    public bool IsStar => StarType != null;
    public bool IsPlanet => PlanetClass != null;
    public bool IsEarthLike => PlanetClass == "Earthlike body";
    public bool IsWaterWorld => PlanetClass == "Water world";
    public bool IsAmmoniaWorld => PlanetClass == "Ammonia world";
    public bool IsTerraformable => TerraformState is "Terraformable" or "Terraforming" or "Terraformed";
    public bool IsValuable => IsEarthLike || IsWaterWorld || IsAmmoniaWorld || IsTerraformable;
    
    public long EstimatedValue => CalculateValue();

    private long CalculateValue()
    {
        if (!IsPlanet) return 0;

        // Base Value
        double baseValue = PlanetClass switch
        {
            "Earthlike body" => 300000,
            "Water world" => 180000,
            "Ammonia world" => 180000,
            "Metal rich body" => 21790,
            "High metal content body" => 9654,
            "Rocky body" => 1000,
            "Icy body" => 1000,
            "Rocky ice body" => 1000,
            "Sudarsky class I gas giant" => 3974,
            "Sudarsky class II gas giant" => 9654,
            "Sudarsky class III gas giant" => 1102,
            "Sudarsky class IV gas giant" => 2101,
            "Sudarsky class V gas giant" => 2101,
            "Gas giant with water based life" => 9654,
            "Gas giant with ammonia based life" => 9654,
            "Helium rich gas giant" => 1102,
            "Helium gas giant" => 1102,
            _ => 1000
        };

        // Mass
        double mass = MassEM ?? 1.0;
        double scaledValue = baseValue * Math.Pow(mass, 0.56);

        // Terraform bonus
        if (IsTerraformable)
            scaledValue += 93328 * Math.Pow(mass, 0.56);

        // First Discover
        if (!WasDiscovered)
            scaledValue *= 2.6;

        // First Map
        if (!WasMapped)
            scaledValue *= 3.3;

        return (long)scaledValue;
    }
}