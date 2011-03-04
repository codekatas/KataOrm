namespace KataOrm.Infrastructure.Container
{
    public class SimpleContainerItemResolver : IContainerItemResolver
    {
        private readonly DependencyResolver _resolver;

        public SimpleContainerItemResolver(DependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public object Resolve()
        {
            return _resolver();
        }
    }
}