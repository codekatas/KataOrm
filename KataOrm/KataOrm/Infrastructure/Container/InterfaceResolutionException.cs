using System;

namespace KataOrm.Infrastructure.Container
{

    public class InterfaceResolutionException : Exception
    {

        public InterfaceResolutionException(Exception innerException, Type typeThatCouldNotBeResolved)
        : base(string.Format("Failed to resolve an implementation of {0}", typeThatCouldNotBeResolved.Name), innerException)
        {
            TypeThatCouldNotBeResolved = typeThatCouldNotBeResolved;
        }

        public Type TypeThatCouldNotBeResolved { get; private set; }
    }
}