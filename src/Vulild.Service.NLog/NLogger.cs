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
    }
}
