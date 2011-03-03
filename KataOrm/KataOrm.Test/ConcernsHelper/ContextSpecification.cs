using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace KataOrm.Test.ConcernsHelper
{
    [TestClass]
    public abstract class ContextSpecification
    {
        [TestInitialize]
        public void Setup()
        {
            EstablishContext();
            InitializeSystemUnderTest();
            Because();
        }

        protected abstract void Because();

        protected virtual void InitializeSystemUnderTest(){}

        protected virtual void EstablishContext(){}

        protected virtual void AfterEachTest(){}

        protected InterfaceType Dependency<InterfaceType>() where InterfaceType : class
        {
            return MockRepository.GenerateMock<InterfaceType>();
        }

        [TestCleanup]
        public void ClearDown()
        {
            AfterEachTest();
        }
    }


    [TestClass]
    public abstract class InstanceContextSpecification<SystemUnderTest> : ContextSpecification
    {
        protected SystemUnderTest Sut;
        protected abstract SystemUnderTest CreateSut();

        protected override void InitializeSystemUnderTest()
        {
            Sut = CreateSut();
        }
    }
}