using System;
using System.Linq;
using EDExplorix.Models.Database;
using EDExplorix.Models.Journal;
using EDExplorix.Services.Journal;

namespace EDExplorix.Services.Database;

public class ExplorationService : IDisposable
{
    private readonly AppDbContext _db;
    private readonly JournalWatcher _watcher;
    private readonly IDisposable _subscription;

    public ExplorationService(AppDbContext db, JournalWatcher watcher)
    {
        _db = db;
        _watcher = watcher;
        _subscription = _watcher.Events.Subscribe(OnJournalEvent);
    }

    private void OnJournalEvent(JournalEvent journalEvent)
    {
        switch (journalEvent)
        {
            case FSDJumpEvent jump:
                HandleFSDJump(jump);
                break;
            case FSSDiscoveryScanEvent fss:
                HandleFSSDiscoveryScan(fss);
                break;
            case ScanEvent scan:
                HandleScan(scan);
                break;
            case FSSBodySignalsEvent signals:
                HandleBodySignals(signals);
                break;
        }
    }

    private void HandleFSDJump(FSDJumpEvent jump)
    {
        var existing = _db.StarSystems.Find(jump.SystemAddress);
        if (existing != null)
            return;

        var system = new StarSystem
        {
            SystemAddress = jump.SystemAddress,
            Name = jump.StarSystem,
            X = jump.StarPos?.Length > 0 ? jump.StarPos[0] : 0,
            Y = jump.StarPos?.Length > 1 ? jump.StarPos[1] : 0,
            Z = jump.StarPos?.Length > 2 ? jump.StarPos[2] : 0,
            FirstVisited = jump.Timestamp
        };

        _db.StarSystems.Add(system);
        _db.SaveChanges();
    }

    private void HandleFSSDiscoveryScan(FSSDiscoveryScanEvent fss)
    {
        var system = _db.StarSystems.Find(fss.SystemAddress);
        if (system == null)
            return;

        system.BodyCount = fss.BodyCount;
        system.NonBodyCount = fss.NonBodyCount;
        _db.SaveChanges();
    }

    private void HandleScan(ScanEvent scan)
    {
        if (scan.IsStar)
            return;

        var system = _db.StarSystems.Find(scan.SystemAddress);
        if (system == null)
            return;

        var existing = _db.Bodies
            .FirstOrDefault(b => b.SystemAddress == scan.SystemAddress
                              && b.BodyId == scan.BodyId);
        if (existing != null)
            return;

        var body = new Body
        {
            BodyId = scan.BodyId,
            SystemAddress = scan.SystemAddress,
            Name = scan.BodyName,
            StarType = scan.StarType,
            PlanetClass = scan.PlanetClass,
            MassEM = scan.MassEM,
            Radius = scan.Radius,
            SurfaceGravity = scan.SurfaceGravity,
            SurfaceTemperature = scan.SurfaceTemperature,
            SurfacePressure = scan.SurfacePressure,
            Landable = scan.Landable,
            TerraformState = scan.TerraformState,
            AtmosphereType = scan.AtmosphereType,
            Volcanism = scan.Volcanism,
            OrbitalPeriod = scan.OrbitalPeriod,
            Eccentricity = scan.Eccentricity,
            WasDiscovered = scan.WasDiscovered,
            WasMapped = scan.WasMapped,
            ScannedAt = scan.Timestamp
        };

        _db.Bodies.Add(body);
        _db.SaveChanges();
    }

    private void HandleBodySignals(FSSBodySignalsEvent signals)
    {
        var body = _db.Bodies
            .FirstOrDefault(b => b.SystemAddress == signals.SystemAddress
                              && b.BodyId == signals.BodyId);
        if (body == null)
            return;

        body.BiologicalSignals = signals.Signals
            .FirstOrDefault(s => s.TypeDisplay == "Biological")?.Count ?? 0;
        body.GeologicalSignals = signals.Signals
            .FirstOrDefault(s => s.TypeDisplay == "Geological")?.Count ?? 0;

        _db.SaveChanges();
    }

    public void Dispose()
    {
        _subscription.Dispose();
        _db.Dispose();
    }
}