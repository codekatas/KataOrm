using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KataOrm.MetaStore;
using KataOrm.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.MetaInfo
{
    [TestClass]
    public class TableInfoTests
    {
        private MetaInfoStore metaInfoStore;

        [TestInitialize]
        public void Setup()
        {
            metaInfoStore = new MetaInfoStore();
            metaInfoStore.BuildMetaInfoForAssembly(Assembly.GetExecutingAssembly());
        }

        [TestMethod]
        public void Should_return_select_statement_with_all_columns_in_Table_Entity()
        {
            //Arrange

            KeyValuePair<Type, TableInfo> tablInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsOne").First();

            string selectStatement = tablInfo.Value.GetSelectTableForAllFields();

            Assert.IsTrue(selectStatement.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase));
            foreach (ColumnInfo columnInfo in tablInfo.Value.ColumnInfos)
            {
                Assert.IsTrue(selectStatement.Contains(columnInfo.Name));
            }
        }

        [TestMethod]
        public void Should_return_select_statement_with_the_reference_key_columns()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").First();

            string selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            foreach (ColumnInfo referenceColumns in tableInfo.Value.References)
            {
                Assert.IsTrue(selectStatement.Contains(referenceColumns.Name));
            }
        }


        [TestMethod]
        public void Should_return_select_statement_with_primary_key_column()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").First();

            string selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            Assert.IsTrue(selectStatement.Contains(tableInfo.Value.PrimaryKey.Name));
        }


        [TestMethod]
        public void Select_statement_should_have_a_FROM_keyword()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsTwo").First();

            string selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            string fromString = "FROM";
            Assert.IsTrue(selectStatement.Contains(fromString));
            Assert.IsTrue(selectStatement.Contains(tableInfo.Value.TableName));
        }

        [TestMethod]
        public void Select_statement_should_exclude_last_comma_before_FROM_keyword()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsThree").First();

            string selectStatement = tableInfo.Value.GetSelectTableForAllFields();
            int commaCount = TextHelper.StringOccurence(selectStatement, ",");
            int colCount = (tableInfo.Value.ColumnInfos.Count() + tableInfo.Value.References.Count());
            if (tableInfo.Value.PrimaryKey != null)
            {
                colCount ++;
            }
            Assert.AreEqual(colCount - 1, commaCount);
        }


        [TestMethod]
        public void Should_generate_a_valid_Insert_statement_when_GetInsertStatement_is_called()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsOne").First();
            string insertStatement = tableInfo.Value.GetInsertStatement();
            Assert.IsTrue(insertStatement.Contains("INSERT INTO"));
        }

        [TestMethod]
        public void Insert_statement_generated_should_contain_the_the_target_TableName()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsOne").First();
            string insertStatement = tableInfo.Value.GetInsertStatement();

            Assert.IsTrue(insertStatement.Contains(tableInfo.Value.TableName));
        }

        [TestMethod]
        public void Should_return_valid_insert_statement_with_correct_parameter_names_for_TableInfo()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsFour").First();
            string insertStatement = tableInfo.Value.GetInsertStatement();

            foreach (ColumnInfo columnInfo in tableInfo.Value.ColumnInfos)
            {
                Assert.IsTrue(insertStatement.Contains("@" + columnInfo.Name));
            }

            foreach (ColumnInfo referenceColInfo in tableInfo.Value.References)
            {
                Assert.IsTrue(insertStatement.Contains("@" + referenceColInfo.Name));
            }
        }


        [TestMethod]
        public void Update_statement_generated_should_contain_the_target_TableName()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsFour").First();
            string insertStatement = tableInfo.Value.GetUpdateStatement();

            Assert.IsTrue(insertStatement.Contains("UPDATE " + "[" + tableInfo.Value.TableName + "]"));

        }


        [TestMethod]
        public void Should_return_valid_update_statement_with_correct_parameter_names_for_TableInfo()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsFour").First();
            string insertStatement = tableInfo.Value.GetUpdateStatement();

            foreach (ColumnInfo columnInfo in tableInfo.Value.ColumnInfos)
            {
                Assert.IsTrue(insertStatement.Contains("@" + columnInfo.Name));
            }

            foreach (ColumnInfo referenceColInfo in tableInfo.Value.References)
            {
                Assert.IsTrue(insertStatement.Contains("@" + referenceColInfo.Name));
            }
        }


        [TestMethod]
        public void Should_return_valid_update_statement_should_contain_a_valid_where_Clause()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsFour").First();
            string insertStatement = tableInfo.Value.GetUpdateStatement();

            Assert.IsTrue(insertStatement.Contains("WHERE"));
            Assert.IsTrue(insertStatement.Contains("WHERE [" + tableInfo.Value.PrimaryKey.Name + "] = @"));
        }

        [TestMethod]
        public void Should_return_a_valid_schema_creation_script_for_a_TableInfo_Entity()
        {
            KeyValuePair<Type, TableInfo> tableInfo =
                    metaInfoStore.TableInfos.Where(x => x.Key.Name == "TableInfoTestsFour").First();
            string createSchemaScript = tableInfo.Value.GetCreateStatement();
            
            Assert.IsTrue(createSchemaScript.Contains("CREATE TABLE [dbo]." + Escape(tableInfo.Value.TableName)));
            foreach (var columnInfo in tableInfo.Value.ColumnInfos)
            {
                Assert.IsTrue(createSchemaScript.Contains(Escape(columnInfo.Name)));
            }
        }

        private string Escape(string value)
        {
            return "[" + value + "]";
        }
    }
}