using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KataOrm.Infrastructure.Container;
using KataOrm.Test.ConcernsHelper;
using KataOrm.Test.SpecHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace KataOrm.Test.Infrastructure.ContainerSpecs
{
    public abstract class Concern_for_a_Simple_Container : InstanceContextSpecification<SimpleContainer>
    {
        protected IDictionary<Type, IContainerItemResolver> _resolvers;

        protected override void EstablishContext()
        {
            _resolvers = new Dictionary<Type, IContainerItemResolver>();
        }

        protected override SimpleContainer CreateSut()
        {
            return new SimpleContainer(_resolvers);
        }
    }

    [TestClass]
    public class When_a_Container_is_told_to_get_an_implementation_of_an_Interface_ : Concern_for_a_Simple_Container
    {
        private IContainerItemResolver _resolver;
        private IDbDataAdapter _result;
        private IDbDataAdapter dataAdapter;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            dataAdapter = Dependency<IDbDataAdapter>();
            _resolver = Dependency<IContainerItemResolver>();
            _resolvers.Add(typeof (IDbDataAdapter), _resolver);
            _resolvers.Add(typeof (SqlDataAdapter), Dependency<IContainerItemResolver>());
            _resolver.Stub(x => x.Resolve()).Return(dataAdapter);
        }

        protected override void Because()
        {
            _result = Sut.GetMeAn<IDbDataAdapter>();
        }

        [TestMethod]
        public void Should_use_the_Container_Item_resolver_for_the_requested_type_to_resolve_the_item()
        {
            Assert.AreEqual(_result, dataAdapter);
        }
    }

    [TestClass]
    public class When_an_unregistered_resolver_for_a_type_is_added_to_the_Simple_Container :
        Concern_for_a_Simple_Container
    {
        private IContainerItemResolver _resolver;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _resolver = Dependency<IContainerItemResolver>();
        }

        protected override void Because()
        {
            Sut.AddResolverFor<SqlDataAdapter>(_resolver);
        }

        [TestMethod]
        public void Should_store_the_Resolver_to_be_used_to_resolve_the_correct_type()
        {
            Assert.AreEqual(_resolvers[typeof (SqlDataAdapter)], _resolver);
        }
    }

    [TestClass]
    public class When_attempting_to_register_a_Resolver_for_a_type_that_already_has_a_type_Registered_for_it :
        Concern_for_a_Simple_Container
    {
        private IContainerItemResolver _resolver;
        private Action action;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _resolver = Dependency<IContainerItemResolver>();
            _resolvers.Add(typeof (IDataAdapter), Dependency<IContainerItemResolver>());
        }

        protected override void Because()
        {
            action = () => Sut.AddResolverFor<IDataAdapter>(_resolver);
        }

        [TestMethod]
        public void Should_throw_a_resolver_already_registered_exception_for_the_specific_type()
        {
            action.ShouldThowAn<ResolverAlreadyRegisteredException>().TypeAlreadyWithAResolver.ShouldBeEqualTo(
                typeof (IDataAdapter));
        }
    }

    [TestClass]
    public class When_a_Container_is_asked_for_a_Resolver_for_a_Type_that_does_not_have_a_Resolver_registered_for_it : Concern_for_a_Simple_Container
    {
        private Action action;

        protected override void Because()
        {
            action = () => Sut.GetMeAn<IDataAdapter>();
        }

        [TestMethod]
        public void Should_Throw_a_ResolverNotRegisteredException()
        {
            action.ShouldThowAn<ResolverNotRegisteredException>().TypeWithNoResolver.ShouldBeEqualTo(
                typeof (IDataAdapter));
        }

    }
}