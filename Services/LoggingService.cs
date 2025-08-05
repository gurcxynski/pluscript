using System.Collections.Concurrent;

namespace PluScript.Services;

public class LoggingService
{
    private readonly ConcurrentQueue<LogEntry> _logs = new();
    private const int MaxLogEntries = 1000;

    public void Log(string message, string category = "General")
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Message = message,
            Category = category
        };

        _logs.Enqueue(entry);

        // Keep only the latest entries
        while (_logs.Count > MaxLogEntries)
        {
            _logs.TryDequeue(out _);
        }
    }

    public IEnumerable<LogEntry> GetLogs()
    {
        return _logs.ToArray().Reverse();
    }

    public void ClearLogs()
    {
        _logs.Clear();
    }
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
