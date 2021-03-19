using System;

namespace JackSParrot.Utils
{
    public enum LogLevel
    {
        Debug,
        Warning,
        Error,
        None
    };

    public abstract class CustomLoggerServiceSO : Service
    {
        public abstract void SetLogLevel(LogLevel level);
        public abstract void LogDebug(string message);
        public abstract void LogError(string message);
        public abstract void LogWarning(string message);
    }
}