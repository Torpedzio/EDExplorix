using System;
using System.Text.Json.Serialization;

namespace EDExplorix.Models.Journal;

public abstract class JournalEvent
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;
}