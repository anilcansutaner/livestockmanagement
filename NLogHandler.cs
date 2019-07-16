using NLog;

public class NLogHandler : ILogHandler
{
    private Logger _logger;

    public static ILogHandler Instance(string loggerName)
    {
        return new NLogHandler(loggerName);
    }

    public NLogHandler(string loggerName)
    {
        _logger = LogManager.GetLogger(loggerName);
    }

    public void LogDebug(string log)
    {
        _logger.Debug(log);
    }

    public void LogError(string log)
    {
        _logger.Error(log);
    }

    public void LogFatal(string log)
    {
        _logger.Fatal(log);
    }

    public void LogTrace(string log)
    {
        _logger.Trace(log);
    }

    public void LogWarn(string log)
    {
        _logger.Warn(log);
    }
}