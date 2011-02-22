using System;

namespace KataOrm.Infrastructure
{
    public class Log4NetLogFactory : ILogFactory
    {
        public Log4NetLogFactory(ICommand initializationCommand)
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogFor(Type Item_that_requires_logging)
        {
            throw new NotImplementedException();
        }
    }

}