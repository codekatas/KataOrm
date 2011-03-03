using System;

namespace KataOrm.Infrastructure.Container
{

    public class ResolverNotRegisteredException : Exception
    {
        public ResolverNotRegisteredException(Type typeWithNoResolver)
        {
            TypeWithNoResolver = typeWithNoResolver;
        }

        public Type TypeWithNoResolver { get; private set; }
    }
}