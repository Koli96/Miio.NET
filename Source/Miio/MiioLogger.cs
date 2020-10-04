using NLog;

namespace Miio
{
    public class MiioLogger : IMiioLogger
    {
        private readonly ILogger _logger; 
        public MiioLogger()
        {
            var logTarget = new NLog.Targets.DebuggerTarget();
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logTarget);
            LogManager.Configuration = config;

            _logger = LogManager.GetLogger("MiioEngine");
        }

        public void Debug(LogEntry logEntry)
        {
            Log(LogLevel.Debug, logEntry);
        }

        public void Error(LogEntry logEntry)
        {
            Log(LogLevel.Error, logEntry);
        }

        public void Fatal(LogEntry logEntry)
        {
            Log(LogLevel.Fatal, logEntry);
        }

        public void Info(LogEntry logEntry)
        {
            Log(LogLevel.Info, logEntry);
        }

        public void Warning(LogEntry logEntry)
        {
            Log(LogLevel.Warn, logEntry);
        }

        public void Log(LogLevel logLevel, LogEntry logEntry)
        {
            _logger.Log(logLevel, $"[Miio.NET] | {logEntry.Ip} | {logEntry.Message} | {logEntry.DeviceToken} | {logEntry.Command} | {logEntry.OutPacket} | {logEntry.InPacket}");
        }
    }
}
