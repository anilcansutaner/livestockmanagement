public interface ILogHandler
{

    void LogDebug(string log);
    void LogTrace(string log);
    void LogWarn(string log);
    void LogError(string log);
    void LogFatal(string log);

}