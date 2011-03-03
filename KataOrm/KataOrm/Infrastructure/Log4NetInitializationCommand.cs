using System.Xml;
using log4net.Config;

namespace KataOrm.Infrastructure
{
    public class Log4NetInitializationCommand : ICommand
    {
        private readonly XmlElement _log4NetConfigurationElement;

        public Log4NetInitializationCommand(XmlElement log4NetConfigurationElement)
        {
            _log4NetConfigurationElement = log4NetConfigurationElement;
        }

        public void run()
        {
            XmlConfigurator.Configure(_log4NetConfigurationElement);
        }
    }
}