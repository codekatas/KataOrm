using KataOrm.Infrastructure;
using KataOrm.Test.ConcernsHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace KataOrm.Test.Infrastructure.Log4NetSpecs
{
    [TestClass]
    public class Log4NetFactorySpecs : InstanceContextSpecification<ILogFactory>
    {
        protected ICommand initializationCommand;
        private ILogger result;

        protected override void Because()
        {
            result = Sut.CreateLogFor(GetType());
        }

        protected override void EstablishContext()
        {
            initializationCommand = Dependency<ICommand>();
        }

        protected override ILogFactory CreateSut()
        {
            return new Log4NetLogFactory(initializationCommand);
        }

        [TestMethod]
        public void Should_tell_the_Log_4_net_initialization_command_to_run()
        {
            initializationCommand.AssertWasCalled(x => x.run());
        }
    }

    [TestClass]
    public class When_a_log_4_net_factory_is_told_to_create_the_first_log_bound_to_a_type : Log4NetFactorySpecs
    {
        private ILogger result;

        protected override void Because()
        {
            result = Sut.CreateLogFor(GetType());
        }

        [TestMethod]
        public void Should_tell_the_Log_4_net_initialization_command_to_run()
        {
            initializationCommand.AssertWasCalled(x => x.run());
        }
    }
}