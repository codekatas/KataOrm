using System;

namespace KataOrm.Infrastructure
{
    public class TextLoggerFactory : ILogFactory
    {
        public ILogger CreateLogFor(Type Item_that_requires_logging)
        {
            return new TextLogger(Console.Out);
        }
    }
}