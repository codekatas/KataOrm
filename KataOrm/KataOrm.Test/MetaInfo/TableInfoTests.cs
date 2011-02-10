using System;
using System.Reflection;
using System.Linq;
using KataOrm.MetaStore;
using KataOrm.Test.Helper;
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


        [TestMethod]
        public void Should_return_select_statement_with_primary_key_column()
        {
            var metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());
            var tableInfo = metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").First();

            var selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            Assert.IsTrue(selectStatement.Contains(tableInfo.Value.PrimaryKey.Name));
        }


        [TestMethod]
        public void Select_statement_should_have_a_FROM_keyword()
        {
            var metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());
            var tableInfo = metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").First();

            var selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            string fromString = "FROM";
            Assert.IsTrue(selectStatement.Contains(fromString)); 
            Assert.IsTrue(selectStatement.Contains(tableInfo.Value.TableName)); 
        }

        [TestMethod]
        public void Select_statement_should_exclude_last_comma_before_FROM_keyword()
        {
            var metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());
            var tableInfo = metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsThree").First();

            var selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            var commaCount = TextHelper.StringOccurence(selectStatement, ",");
            var colCount = (tableInfo.Value.ColumnInfos.Count() + tableInfo.Value.References.Count());
            if(tableInfo.Value.PrimaryKey != null )
            {
                colCount ++;
            }
            Assert.AreEqual(colCount -1 , commaCount);

        }
    }
}