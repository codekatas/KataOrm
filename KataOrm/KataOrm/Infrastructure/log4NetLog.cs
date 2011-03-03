using System;
using log4net;

namespace KataOrm.Infrastructure
{
    public class log4NetLog : ILogger
    {
        private readonly log4net.ILog _underlyingLog;

        public log4NetLog(log4net.ILog logger)
        {
            _underlyingLog = logger;
        }

        public void Log(string messageToLog)
        {
            _underlyingLog.Info(messageToLog);
        }
    }
}