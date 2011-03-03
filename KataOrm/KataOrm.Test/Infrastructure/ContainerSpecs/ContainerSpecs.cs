using System;
using System.Data;
using KataOrm.Infrastructure.Container;
using KataOrm.Test.SpecHelpers;
using KataOrm.Test.ConcernsHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace KataOrm.Test.Infrastructure.ContainerSpecs
{
    public abstract class Concern_For_Container : ContextSpecification
    {
        protected IContainer underlyingContainer;

        protected override void EstablishContext()
        {
            underlyingContainer = Dependency<IContainer>();
            Container.InitializeWith(underlyingContainer);
        }
    }

    [TestClass]
    public class When_a_Container_is_told_to_get_an_implementation_of_an_interface : Concern_For_Container
    {
        private IDataAdapter _result;
        private IDataAdapter _dataAdapter;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _dataAdapter = Dependency<IDataAdapter>();
            underlyingContainer.Stub(x => x.GetMeAn<IDataAdapter>()).Return(_dataAdapter);
        }
        protected override void Because()
        {
            _result = Container.GetMeAnImplementationOf<IDataAdapter>();
        }

        [TestMethod]
        public void Should_get_me_an_implementation_of_the_Interface()
        {
           _result.ShouldBeEqualTo(_dataAdapter);
        }
    }

    [TestClass]
    public class When_then_underlying_container_throws_an_exception_when_trying_to_get_an_Interface_implementation : Concern_For_Container
    {
        private Action action;
        private Exception _innerException = new Exception();

        protected override void EstablishContext()
        {
            base.EstablishContext();
            underlyingContainer.Stub(x => x.GetMeAn<IDataAdapter>()).Throw(_innerException);
        }
        protected override void Because()
        {
            action = () => Container.GetMeAnImplementationOf<IDataAdapter>();
        }

        [TestMethod]
        public void Should_throw_an_interface_resolution_exception_with_access_to_the_Inner_Exception_of_the_container()
        {
            var interfaceResolutionException = action.ShouldThowAn<InterfaceResolutionException>();
            interfaceResolutionException.InnerException.ShouldBeEqualTo(_innerException);
            interfaceResolutionException.TypeThatCouldNotBeResolved.ShouldBeEqualTo(typeof(IDataAdapter));
        }
    }

}