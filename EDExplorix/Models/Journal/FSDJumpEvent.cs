using System.Text.Json.Serialization;

namespace EDExplorix.Models.Journal;

public class FSDJumpEvent : JournalEvent
{
    [JsonPropertyName("StarSystem")]
    public string StarSystem { get; set; } = string.Empty;
    [JsonPropertyName("SystemAddress")]
    public long SystemAddress { get; set; }
    [JsonPropertyName("StarPos")] 
    public float[] StarPos { get; set; } = [];
    [JsonPropertyName("SystemAllegiance")]
    public string? SystemAllegiance { get; set; }
    [JsonPropertyName("SystemEconomy_Localised")]
    public string? Economy { get; set; }
    [JsonPropertyName("SystemSecondEconomy_Localised")]
    public string? SecondEconomy { get; set; }
    [JsonPropertyName("SystemSecurity_Localised")]
    public string? Security { get; set; }
    [JsonPropertyName("Population")]
    public long Population { get; set; }
}