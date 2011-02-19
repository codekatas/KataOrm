using System.Reflection;
using KataOrm.MetaStore;
using KataOrm.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.SchemaSetup
{
    [TestClass]
    public class AssemblyCreateSchema
    {
        [TestMethod]
        public void Should_create_SQL_Server_tables_for_each_TableInfo_entity_in_Assembly()
        {
            Assembly assembly = Assembly.LoadFrom(@"D:\Development\KataOrm\KataOrm\KataTestAssembly\bin\Debug\KataTestAssembly.dll");
            MetaInfoStore metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(assembly);

            KataSchemaManager kataSchemaManager = new KataSchemaManager(assembly,new HardCodedTestConfigurationSettings());
            kataSchemaManager.CreateSchema();

            Assert.IsTrue(kataSchemaManager.SuccessfullyCreatedTables.Count == metaInfoStore.TableInfos.Count);
        }
    }
}