using System;
using log4net;

namespace KataOrm.Infrastructure
{
    public class Log4NetLogFactory : ILogFactory
    {
        private ICommand _initializationCommand;
        private bool _isInitialized;

        public Log4NetLogFactory(ICommand initializationCommand)
        {
         _initializationCommand =    initializationCommand;
        }

        public ILogger CreateLogFor(Type Item_that_requires_logging)
        {
            if(!_isInitialized)_initializationCommand.run();
            _isInitialized = true;
            return new log4NetLog(LogManager.GetLogger(Item_that_requires_logging));
        }
    }
}