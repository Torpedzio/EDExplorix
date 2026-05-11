using EDExplorix.Models.Journal;
using EDExplorix.Services.Database;
using EDExplorix.Services.Journal;
using Microsoft.EntityFrameworkCore;

namespace EDExplorix.Tests;

public class ExplorationServiceTest
{
    private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void HandleFSDJump_CreatesNewSystem()
    {
        var db = CreateInMemoryDb();
        var watcher = new JournalWatcher();
        var service = new ExplorationService(db, watcher);

        watcher.SimulateEvent(new FSDJumpEvent
        {
            StarSystem = "Colonia",
            SystemAddress = 123456,
            StarPos = [-9530.5f, -910.28125f, 19808.125f],
            Timestamp = DateTime.UtcNow
        });

        var system = db.StarSystems.Find((long)123456);
        Assert.NotNull(system);
        Assert.Equal("Colonia", system.Name);
    }

    [Fact]
    public void HandleFSDJump_DoesNotDuplicate()
    {
        var db = CreateInMemoryDb();
        var watcher = new JournalWatcher();
        var service = new ExplorationService(db, watcher);

        var jump = new FSDJumpEvent
        {
            StarSystem = "Colonia",
            SystemAddress = 123456,
            StarPos = [-9530.5f, -910.28125f, 19808.125f],
            Timestamp = DateTime.UtcNow
        };

        watcher.SimulateEvent(jump);
        watcher.SimulateEvent(jump);

        Assert.Equal(1, db.StarSystems.Count());
    }

    [Fact]
    public void HandleScan_AddsPlanetToSystem()
    {
        var db = CreateInMemoryDb();
        var watcher = new JournalWatcher();
        var service = new ExplorationService(db, watcher);

        watcher.SimulateEvent(new FSDJumpEvent
        {
            StarSystem = "Colonia",
            SystemAddress = 123456,
            StarPos = [-9530.5f, -910.28125f, 19808.125f],
            Timestamp = DateTime.UtcNow
        });

        watcher.SimulateEvent(new ScanEvent
        {
            BodyName = "Colonia 2",
            BodyId = 2,
            SystemAddress = 123456,
            PlanetClass = "Earthlike body",
            MassEM = 0.89f,
            WasDiscovered = false,
            WasMapped = false,
            Timestamp = DateTime.UtcNow
        });

        var body = db.Bodies.FirstOrDefault(b => b.SystemAddress == 123456);
        Assert.NotNull(body);
        Assert.Equal("Earthlike body", body.PlanetClass);
        Assert.True(body.IsEarthLike);
    }

    [Fact]
    public void HandleScan_IgnoresStars()
    {
        var db = CreateInMemoryDb();
        var watcher = new JournalWatcher();
        var service = new ExplorationService(db, watcher);

        watcher.SimulateEvent(new FSDJumpEvent
        {
            StarSystem = "Colonia",
            SystemAddress = 123456,
            StarPos = [-9530.5f, -910.28125f, 19808.125f],
            Timestamp = DateTime.UtcNow
        });

        watcher.SimulateEvent(new ScanEvent
        {
            BodyName = "Colonia A",
            BodyId = 1,
            SystemAddress = 123456,
            StarType = "G",
            Timestamp = DateTime.UtcNow
        });

        Assert.Equal(0, db.Bodies.Count());
    }
}