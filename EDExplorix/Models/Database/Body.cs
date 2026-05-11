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

    public bool IsStar => StarType != null;
    public bool IsPlanet => PlanetClass != null;
    public bool IsEarthLike => PlanetClass == "Earthlike body";
    public bool IsWaterWorld => PlanetClass == "Water world";
    public bool IsAmmoniaWorld => PlanetClass == "Ammonia world";
    public bool IsTerraformable => TerraformState is "Terraformable" or "Terraforming" or "Terraformed";
    public bool IsValuable => IsEarthLike || IsWaterWorld || IsAmmoniaWorld || IsTerraformable;
}