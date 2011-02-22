using System;

namespace KataOrm.Infrastructure
{
    public interface ILogFactory
    {
        ILogger CreateLogFor(Type Item_that_requires_logging);
    }
}