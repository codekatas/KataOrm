using System;
using System.Reflection;
using System.Linq;
using KataOrm.MetaStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.MetaInfo
{
    [TestClass]
    public class TableInfoTests
    {
        [TestMethod]
        public void Should_return_select_statement_with_all_columns_in_Table_Entity()
        {
            
            //Arrange
            var metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());
            var tablInfo = metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsOne").First();

            var selectStatement = tablInfo.Value.GetSelectTableForAllFields();

            Assert.IsTrue(selectStatement.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase));
            foreach (var columnInfo in tablInfo.Value.ColumnInfos)
            {
                Assert.IsTrue(selectStatement.Contains(columnInfo.Name));
            }
        }

        [TestMethod]
        public void Should_return_select_statement_with_the_reference_key_columns()
        {
            var metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());
            var tableInfo = metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").First();

            var selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            foreach (var referenceColumns in tableInfo.Value.References)
            {
                Assert.IsTrue(selectStatement.Contains(referenceColumns.Name));
            }
        }
    }
}