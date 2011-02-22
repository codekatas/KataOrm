using System;
using KataOrm.Infrastructure;
using KataOrm.Test.ConcernsHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.Log4NetSpecs
{

    [TestClass]
    public abstract class Log4NetFactorySpecs : InstanceContextSpecification<ILogFactory>
    {
        protected ICommand initializationCommand;
         
        protected override void EstablishContext()
        {
            initializationCommand = Dependency<ICommand>();
        }

        protected override ILogFactory CreateSut()
        {
            return new Log4NetLogFactory(initializationCommand);
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
        }
    }
}