using NLog;

namespace Miio
{
    public interface IMiioLogger 
    {
        void Debug(LogEntry logEntry);
        void Info(LogEntry logEntry);
        void Warning(LogEntry logEntry);
        void Error(LogEntry logEntry);
        void Fatal(LogEntry logEntry);
        void Log(LogLevel logLevel, LogEntry logEntry);
    }
}
