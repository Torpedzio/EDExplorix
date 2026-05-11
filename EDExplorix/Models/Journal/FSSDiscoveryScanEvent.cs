using System.Text.Json.Serialization;

namespace EDExplorix.Models.Journal;

public class FSSDiscoveryScanEvent: JournalEvent
{
    [JsonPropertyName("Progress")]
    float Progress { get; set; }
    [JsonPropertyName("BodyCount")]
    public int BodyCount { get; set; }
    [JsonPropertyName("NonBodyCount")]
    public int NonBodyCount { get; set; }
    [JsonPropertyName("SystemName")]
    public string SystemName { get; set; } = string.Empty;
    [JsonPropertyName("SystemAddress")]
    public long SystemAddress { get; set; }
}