using System.Linq;
using System.Reflection;
using KataOrm.MetaStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.MetaInfo
{
    [TestClass]
    public class MetaInfoStoreTests
    {
        [TestMethod]
        public void Should_load_table_info_items_when_asked_to_build_metainfo_for_Assemble()
        {
            //Arrange
            var metaInfoStore = new MetaInfoStore();

            //Act
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());

            //Assert

            int resultCount = metaInfoStore.TableInfos.Where(x => x.Key.Name.Contains("TableInfoTests")).Count();
            Assert.AreEqual(3, resultCount);
        }

        [TestMethod]
        public void Should_load_column_info_items_for_TableInfo_when_asked_to_build_metainfo_for_Assembly()
        {
            //Arrange
            var metaInfoStore = new MetaInfoStore();

            //Act 
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());

            //Assert
            int result = metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsOne").Sum(x => x.Value.ColumnInfos.Count()); 
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void Should_load_the_PrimaryKey_column_for_TableInfo_when_asked_to_build_MetaInfo_for_Assembly()
        {
            //Arrange
            var metaInfoStore = new MetaInfoStore();

            //Act
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());

            //Assert
            var primaryKey =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsOne").First().Value.PrimaryKey;
            Assert.AreEqual("TableKey", primaryKey.Name);
        }

        [TestMethod]
        public void Should_load_the_reference_columns_for_TableInfo_when_asked_to_build_MetaInfo_for_Assembly()
        {
            //Arrange
            var metaInfoStore = new MetaInfoStore();

            //Act
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());

            //Assert
            int result =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").Sum(
                    x => x.Value.References.Count());
            Assert.AreEqual(1, result);
        }

    }
}