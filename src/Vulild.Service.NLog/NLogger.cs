using NLog;
using System;
using Vulild.Service.Attributes;
using Vulild.Service.Log;

namespace Vulild.Service.NLogService
{
    [ServiceOption(Type = typeof(NLoggerOption))]
    public class NLogger : ILogService
    {
        public Option Option { get; set; }
        ILogger logger;
        public NLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void WriteLog(string log)
        {
            this.logger.Info(log);
        }

        public void WriteLog(string log, int level)
        {
            this.logger.Log(LogLevel.FromOrdinal(level), log);
        }

        public void WriteLog(Exception ex, int level = 4)
        {
            this.logger.Log(LogLevel.FromOrdinal(level), ex.ToString());
        }
    }
}
