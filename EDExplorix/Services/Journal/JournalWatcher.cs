using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using EDExplorix.Models.Journal;

namespace EDExplorix.Services.Journal;

public class JournalWatcher : IDisposable
{
    private readonly JournalParser _parser = new();
    private readonly Subject<JournalEvent> _eventSubject = new();
    private FileSystemWatcher? _watcher;
    private string? _currentFile;
    private long _lastPosition;
    private Timer? _pollingTimer;
    private long _lastFileSize;

    public IObservable<JournalEvent> Events => _eventSubject;

    public void Start(string journalFolder)
    {
        _currentFile = Directory
            .GetFiles(journalFolder, "Journal.*.log")
            .OrderByDescending(f => f)
            .FirstOrDefault();

        if (_currentFile != null)
        {
            ReadToEnd(_currentFile);
            _lastFileSize = new FileInfo(_currentFile).Length;
        }

        _watcher = new FileSystemWatcher(journalFolder)
        {
            Filter = "Journal.*.log",
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
            EnableRaisingEvents = true
        };

        //_watcher.Changed += OnFileChanged;
        //_watcher.Created += OnFileCreated;
        
        _pollingTimer = new Timer(PollFile, null, 2000, 2000);
    }
    
    private void PollFile(object? state)
    {
        if (_currentFile == null) return;

        var size = new FileInfo(_currentFile).Length;
        if (size > _lastFileSize)
        {
            _lastFileSize = size;
            ReadNewLines(_currentFile);
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _currentFile = e.FullPath;
        _lastPosition = 0;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"File changed: {e.FullPath}, current: {_currentFile}");
        if (e.FullPath != _currentFile)
            return;

        ReadNewLines(_currentFile);
    }

    private void ReadToEnd(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);

        _lastPosition = stream.Length;
    }

    private void ReadNewLines(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.Seek(_lastPosition, SeekOrigin.Begin);

        using var reader = new StreamReader(stream);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            Console.WriteLine($"New line: {line[..Math.Min(80, line.Length)]}");
            var journalEvent = _parser.ParseLine(line);
            if (journalEvent != null)
            {
                Console.WriteLine($"Publishing event: {journalEvent.Event}");
                _eventSubject.OnNext(journalEvent);
            }
        }

        _lastPosition = stream.Position;
    }

    public void Dispose()
    {
        _pollingTimer?.Dispose();
        _watcher?.Dispose();
        _eventSubject.OnCompleted();
        _eventSubject.Dispose();
    }
    
    //tests
    public void SimulateEvent(JournalEvent journalEvent)
    {
        _eventSubject.OnNext(journalEvent);
    }
}