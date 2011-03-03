using System;

namespace KataOrm.Infrastructure.Container
{
    public class ResolverAlreadyRegisteredException : Exception
    {
        public ResolverAlreadyRegisteredException(Type typeAlreadyWithAResolver)
        {
            TypeAlreadyWithAResolver = typeAlreadyWithAResolver;
        }


        public Type TypeAlreadyWithAResolver { get; private set; }
    }
}