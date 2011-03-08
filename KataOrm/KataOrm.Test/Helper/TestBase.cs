using System;
using System.IO;
using System.Xml;
using KataOrm.MetaStore;
using System.Reflection;
using KataOrm.Infrastructure;
using System.Collections.Generic;
using KataOrm.Infrastructure.Container;

namespace KataOrm.Test.Helper
{
    public class TestBase
    {
        private Assembly _assembly;
        private MetaInfoStore _metaInfoStore;

        public TestBase()
        {
            var simpleContainer = new SimpleContainer(new Dictionary<Type, IContainerItemResolver>());
            simpleContainer.AddResolverFor<ILogFactory>(new SimpleContainerItemResolver(CreateLog4NetFactory));

            Container.InitializeWith(simpleContainer);
            string targetAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KataTestAssembly.dll");
            targetAssemblyPath = targetAssemblyPath.Replace("KataOrm.Test", "KataTestAssembly");

            _assembly = Assembly.LoadFrom(targetAssemblyPath);
            _metaInfoStore = new MetaInfoStore();
            Log.BoundTo(_metaInfoStore).Log("Initial binding to test MetaInfoStore");

            _metaInfoStore.BuildMetaInfoForAssembly(_assembly);
        }

        public MetaInfoStore GetMetaInfo()
        {
            return _metaInfoStore;
        }

        private object CreateLog4NetFactory()
        {
            var log4NetInitalizeCommand = new Log4NetInitializationCommand(load_log_4_net_config_element());
            return new Log4NetLogFactory(log4NetInitalizeCommand);
        }

        private static XmlElement load_log_4_net_config_element()
        {
            var document = new XmlDocument();
            document.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config.xml"));
            return document.DocumentElement;
        }
    }
}