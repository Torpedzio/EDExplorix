using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using EDExplorix.Models.Journal;

namespace EDExplorix.Services.Journal;

public class JournalParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JournalEvent? ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;
        
        string? eventType = null;
        
        try
        {
            var node = JsonNode.Parse(line);
            eventType = node?["event"]?.GetValue<string>();

            JournalEvent? result = eventType switch
            {
                "FSDJump" => JsonSerializer.Deserialize<FSDJumpEvent>(line, Options),
                "FSSDiscoveryScan" => JsonSerializer.Deserialize<FSSDiscoveryScanEvent>(line, Options),
                "Scan" => JsonSerializer.Deserialize<ScanEvent>(line, Options),
                "FSSBodySignals" => JsonSerializer.Deserialize<FSSBodySignalsEvent>(line, Options),
                "SAAScanComplete" => JsonSerializer.Deserialize<SAAScanCompleteEvent>(line, Options),
                _ => null
            };
            Console.WriteLine($"Parsed '{eventType}' -> {result?.GetType().Name ?? "null"}");
            return result;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"ParseLine error for event '{eventType}': {ex.Message}");
            return null;
        }
    }
}