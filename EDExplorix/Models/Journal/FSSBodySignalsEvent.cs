using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EDExplorix.Models.Journal;

public class FSSBodySignalsEvent : JournalEvent
{
    [JsonPropertyName("BodyName")]
    public string BodyName { get; set; } = string.Empty;

    [JsonPropertyName("BodyID")]
    public int BodyId { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long SystemAddress { get; set; }

    [JsonPropertyName("Signals")]
    public List<BodySignal> Signals { get; set; } = [];
}

public class BodySignal
{
    [JsonPropertyName("Type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("Count")]
    public int Count { get; set; }

    public string TypeDisplay => Type switch
    {
        "$SAA_SignalType_Biological;" => "Biological",
        "$SAA_SignalType_Geological;" => "Geological",
        "$SAA_SignalType_Human;" => "Human",
        "$SAA_SignalType_Thargoid;" => "Thargoid",
        _ => Type
    };
}