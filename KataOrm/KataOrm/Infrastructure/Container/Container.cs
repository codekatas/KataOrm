using System;

namespace KataOrm.Infrastructure.Container
{
    public static class Container
    {
        private static IContainer _underlyingContainer;

        public static void InitializeWith(IContainer underlyingContainer)
        {
            _underlyingContainer = underlyingContainer;
        }

        public static Interface GetMeAnImplementationOf<Interface>()
        {
            try
            {
                return _underlyingContainer.GetMeAn<Interface>();
            }
            catch (Exception ex)
            {
                throw new InterfaceResolutionException(ex, typeof (Interface));
            }
        }
    }
}