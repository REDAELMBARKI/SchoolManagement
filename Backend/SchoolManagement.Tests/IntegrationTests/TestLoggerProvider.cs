using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SchoolManagement.Tests.IntegrationTests;

/// <summary>
/// Logger provider that collects logs in memory for test inspection
/// </summary>
public class TestLoggerProvider : ILoggerProvider
{
    private readonly LogLevel _minLevel;
    public static ConcurrentQueue<string> Logs { get; } = new();

    public TestLoggerProvider(LogLevel minLevel = LogLevel.Debug)
    {
        _minLevel = minLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(categoryName, _minLevel);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }

    public static void ClearLogs()
    {
        Logs.Clear();
    }

    public static string GetAllLogs()
    {
        return string.Join(Environment.NewLine, Logs);
    }
}

/// <summary>
/// Logger that writes to static collection
/// </summary>
public class TestLogger : ILogger
{
    private readonly string _categoryName;
    private readonly LogLevel _minLevel;

    public TestLogger(string categoryName, LogLevel minLevel)
    {
        _categoryName = categoryName;
        _minLevel = minLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minLevel;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        try
        {
            var message = formatter(state, exception);
            var logLevelString = GetLogLevelString(logLevel);
            var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
            
            var logEntry = $"[{timestamp}] [{logLevelString}] {_categoryName}: {message}";
            TestLoggerProvider.Logs.Enqueue(logEntry);
            
            // Also write to console for immediate visibility
            Console.WriteLine(logEntry);
            
            if (exception != null)
            {
                var exceptionEntry = $"Exception: {exception}";
                TestLoggerProvider.Logs.Enqueue(exceptionEntry);
                Console.WriteLine(exceptionEntry);
            }
        }
        catch
        {
            // Silently ignore errors
        }
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO ",
            LogLevel.Warning => "WARN ",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRIT ",
            LogLevel.None => "NONE ",
            _ => logLevel.ToString().ToUpper()
        };
    }
}
