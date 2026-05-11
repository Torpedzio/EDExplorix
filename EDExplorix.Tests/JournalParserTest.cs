using EDExplorix.Models.Journal;
using EDExplorix.Services.Journal;

namespace EDExplorix.Tests;

public class JournalParserTest
{
    private readonly JournalParser _parser = new();

    [Fact]
    public void ParseLine_FSDJump_ReturnsCorrectEvent()
    {
        var line = """
            {"timestamp":"2026-05-10T18:00:00Z","event":"FSDJump","StarSystem":"Colonia",
            "SystemAddress":3238296097059,"StarPos":[-9530.5,-910.28125,19808.125]}
            """;

        var result = _parser.ParseLine(line);

        Assert.IsType<FSDJumpEvent>(result);
        var jump = (FSDJumpEvent)result;
        Assert.Equal("Colonia", jump.StarSystem);
        Assert.Equal(3238296097059, jump.SystemAddress);
    }

    [Fact]
    public void ParseLine_FSSDiscoveryScan_ReturnsCorrectEvent()
    {
        var line = """
            {"timestamp":"2026-05-10T18:01:00Z","event":"FSSDiscoveryScan","Progress":1.0,
            "BodyCount":5,"NonBodyCount":2,"SystemName":"Colonia","SystemAddress":3238296097059}
            """;

        var result = _parser.ParseLine(line);

        Assert.IsType<FSSDiscoveryScanEvent>(result);
        var fss = (FSSDiscoveryScanEvent)result;
        Assert.Equal(5, fss.BodyCount);
        Assert.Equal(2, fss.NonBodyCount);
    }

    [Fact]
    public void ParseLine_Scan_Planet_ReturnsCorrectEvent()
    {
        var line = """
            {"timestamp":"2026-05-10T18:02:00Z","event":"Scan","ScanType":"Detailed",
            "BodyName":"Colonia 2","BodyID":2,"SystemAddress":3238296097059,
            "PlanetClass":"Earthlike body","MassEM":0.89,"SurfaceTemperature":288.0,
            "TerraformState":"","Landable":false,"WasDiscovered":false,"WasMapped":false}
            """;

        var result = _parser.ParseLine(line);

        Assert.IsType<ScanEvent>(result);
        var scan = (ScanEvent)result;
        Assert.Equal("Earthlike body", scan.PlanetClass);
        Assert.True(scan.IsPlanet);
        Assert.False(scan.IsStar);
        //Assert.True(scan.IsEarthLike); // właściwość z Body - sprawdzimy osobno
    }

    [Fact]
    public void ParseLine_UnknownEvent_ReturnsNull()
    {
        var line = """{"timestamp":"2026-05-10T18:00:00Z","event":"Docked","StationName":"Jameson"}""";
        var result = _parser.ParseLine(line);
        Assert.Null(result);
    }

    [Fact]
    public void ParseLine_EmptyLine_ReturnsNull()
    {
        var result = _parser.ParseLine("");
        Assert.Null(result);
    }

    [Fact]
    public void ParseLine_InvalidJson_ReturnsNull()
    {
        var result = _parser.ParseLine("nie jestem jsonem");
        Assert.Null(result);
    }
}