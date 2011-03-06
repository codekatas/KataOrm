using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Xml;
using KataOrm.Infrastructure;
using KataOrm.Infrastructure.Container;
using KataOrm.MetaStore;
using KataOrm.Test.ConcernsHelper;
using KataOrm.Test.Helper;
using KataOrm.Test.SpecHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.SchemaSetup
{

    public abstract class Concerning_Kata_Schema_Manager : InstanceContextSpecification<IKataSchemaManager>
    {
        private Assembly _assembly;
        private MetaInfoStore _metaInfoStore;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            SimpleContainer simpleContainer = new SimpleContainer(new Dictionary<Type, IContainerItemResolver>());
            simpleContainer.AddResolverFor<ILogFactory>(new SimpleContainerItemResolver(CreateLog4NetFactory));

            Container.InitializeWith(simpleContainer);
            var targetAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KataTestAssembly.dll");
            targetAssemblyPath = targetAssemblyPath.Replace("KataOrm.Test", "KataTestAssembly");

            _assembly = Assembly.LoadFrom(targetAssemblyPath);
            _metaInfoStore = new MetaInfoStore();
            Log.BoundTo(_metaInfoStore).Log("Initial binding to test MetaInfoStore");

            _metaInfoStore.BuildMetaInfoForAssembly(_assembly);
        }

        protected override IKataSchemaManager CreateSut()
        {
            return new KataSchemaManager(_assembly, new HardCodedTestConfigurationSettings());
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

        [TestMethod]
        public void Should_create_schema_table_for_each_Table_info_entity_in_assembly_when_asked_to()
        {
            Sut.CreateSchema();
            Assert.IsTrue(Sut.SuccessfullyCreatedTables.Count == _metaInfoStore.TableInfos.Count);                    
        }

        [TestMethod]
        public void Should_delete_all_schema_from_database_when_asked_to()
        {
           Sut.DeleteSchema();
           GetSchemaObjectCount().ShouldBeEqualTo(0);
        }

        private int GetSchemaObjectCount()
        {

            var connection = new SqlConnection(new HardCodedTestConfigurationSettings().ConnectionSettings);
            using(var command = new SqlCommand())
            {
                
            }
        }
    }


    [TestClass]
    public class AssemblyCreateSchema
    {
        [TestMethod]
        public void Should_create_SQL_Server_tables_for_each_TableInfo_entity_in_Assembly()
        {
            //Initialize logging
            SimpleContainer simpleContainer  = new SimpleContainer(new Dictionary<Type, IContainerItemResolver>());
            simpleContainer.AddResolverFor<ILogFactory>(new SimpleContainerItemResolver(CreateLog4NetFactory));

            Container.InitializeWith(simpleContainer);
            var targetAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KataTestAssembly.dll");
            targetAssemblyPath = targetAssemblyPath.Replace("KataOrm.Test", "KataTestAssembly");

            var assembly = Assembly.LoadFrom(targetAssemblyPath);
            var metaInfoStore = new MetaInfoStore();
            Log.BoundTo(metaInfoStore).Log("Initial binding to test MetaInfoStore");

            metaInfoStore.BuildMetaInfoForAssembly(assembly);

            var kataSchemaManager = new KataSchemaManager(assembly,new HardCodedTestConfigurationSettings());
            kataSchemaManager.CreateSchema();

            Assert.IsTrue(kataSchemaManager.SuccessfullyCreatedTables.Count == metaInfoStore.TableInfos.Count);
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

        private string GetLocalLogingFile()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        }

    }
}