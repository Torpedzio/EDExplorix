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

        try
        {
            var node = JsonNode.Parse(line);
            var eventType = node?["event"]?.GetValue<string>();

            return eventType switch
            {
                "FSDJump" => JsonSerializer.Deserialize<FSDJumpEvent>(line, Options),
                "FSSDiscoveryScan" => JsonSerializer.Deserialize<FSSDiscoveryScanEvent>(line, Options),
                "Scan" => JsonSerializer.Deserialize<ScanEvent>(line, Options),
                _ => null
            };
        }
        catch (JsonException)
        {
            return null;
        }
    }
}