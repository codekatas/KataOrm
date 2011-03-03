using System;
using System.Collections.Generic;

namespace KataOrm.Infrastructure.Container
{
    public class SimpleContainer : IContainer
    {
        private readonly IDictionary<Type, IContainerItemResolver> _resolvers;

        public SimpleContainer(IDictionary<Type, IContainerItemResolver> resolvers)
        {
            _resolvers = resolvers;
        }

        #region IContainer Members

        public Interface GetMeAn<Interface>()
        {
            try
            {
                IContainerItemResolver result = _resolvers[typeof(Interface)];
                return (Interface)result.Resolve();
            }
            catch (KeyNotFoundException ex)
            {
                
                throw new ResolverNotRegisteredException(typeof(Interface));
            }
            
        }

        #endregion

        public void AddResolverFor<T>(IContainerItemResolver resolver)
        {
            if (_resolvers.ContainsKey(typeof (T)))
            {
                throw new ResolverAlreadyRegisteredException(typeof (T));
            }
            _resolvers.Add(typeof (T), resolver);
        }
    }
}