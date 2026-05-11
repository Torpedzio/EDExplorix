using System.Text.Json.Serialization;

namespace EDExplorix.Models.Journal;

public class SAAScanCompleteEvent : JournalEvent
{
    [JsonPropertyName("BodyName")]
    public string BodyName { get; set; } = string.Empty;

    [JsonPropertyName("BodyID")]
    public int BodyId { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long SystemAddress { get; set; }
}