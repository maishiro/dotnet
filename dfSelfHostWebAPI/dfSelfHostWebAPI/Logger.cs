using log4net;
using System;

public interface ILogger<T>
{
    void Debug(string message);
    void Info(string message);
    void Error(string message, Exception ex = null);
}

public class Log4NetLogger<T> : ILogger<T>
{
    private readonly ILog _log;

    public Log4NetLogger()
    {
        _log = LogManager.GetLogger( typeof( T ) );
    }

    public void Debug(string message) => _log.Debug(message);
    public void Info(string message) => _log.Info(message);
    public void Error(string message, Exception ex = null) => _log.Error(message, ex);
}