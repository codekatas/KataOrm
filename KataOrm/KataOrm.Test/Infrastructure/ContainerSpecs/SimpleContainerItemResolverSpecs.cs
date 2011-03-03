using System.Data.SqlClient;
using KataOrm.Infrastructure.Container;
using KataOrm.Test.ConcernsHelper;
using KataOrm.Test.SpecHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.Infrastructure.ContainerSpecs
{
    [TestClass]
    public class When_the_SimpleContainerItemResolver_is_told_to_resolve_an_item : InstanceContextSpecification<IContainerItemResolver>
    {
        private object _result;
        private DependencyResolver _resolver;
        private SqlConnection _sqlConnection;

        protected override void EstablishContext()
        {
            _sqlConnection = new SqlConnection();
            _resolver = () => _sqlConnection;
        }

        protected override IContainerItemResolver CreateSut()
        {
            return new SimpleContainerItemResolver(_resolver);
        }

        protected override void Because()
        {
            _result = Sut.Resolve();
        }

        [TestMethod]
        public void Should_use_the_dependency_resolver_delegate_to_resolve_the_item()
        {
            _result.ShouldBeEqualTo(_sqlConnection);
        }
    }
}