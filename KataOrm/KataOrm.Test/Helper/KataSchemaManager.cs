using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using KataOrm.Configuration;
using KataOrm.MetaStore;

namespace KataOrm.Test.Helper
{
    public class KataSchemaManager : IKataSchemaManager
    {
        private readonly Assembly _assembly;
        private readonly IConfigurationSettings _configurationSettings;
        private readonly MetaInfoStore _metaInfoStore;

        public KataSchemaManager(Assembly assembly, IConfigurationSettings configurationSettings)
        {
            SuccessfullyCreatedTables = new List<string>();
            _assembly = assembly;
            _metaInfoStore = new MetaInfoStore();
            _configurationSettings = configurationSettings;
        }

        public List<string> SuccessfullyCreatedTables { get; set; }

        #region IKataSchemaManager Members

        public void CreateSchema()
        {
            _metaInfoStore.BuildMetaInfoForAssembly(_assembly);
            SuccessfullyCreatedTables = new List<string>();
            using (var connection = new SqlConnection(_configurationSettings.ConnectionSettings))
            {
                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = connection;
                    sqlCommand.Connection.Open();
                    //create tables with no reference
                    foreach (var keyValuePair in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() == 0))
                    {
                        CreateTable(sqlCommand, keyValuePair.Value.TableName, keyValuePair.Value.GetCreateStatement(),
                                    SuccessfullyCreatedTables);
                    }

                    CreateTablesWithReferences(sqlCommand, SuccessfullyCreatedTables);
                }
                connection.Close();
            }
        }

        public void DeleteSchema()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetBatchesFromSqlStatement(string sqlStatement)
        {
            int currentPos = 0;
            var result = new List<string>();
            string GoStatement = "GO\r\n";
            while (true)
            {
                int index = sqlStatement.IndexOf(GoStatement, currentPos);
                if (index < 0) return result;
                result.Add(sqlStatement.Substring(currentPos, index - currentPos));
                currentPos = index + 2;
            }
        }

        #endregion

        private void CreateTablesWithReferences(SqlCommand sqlCommand, List<string> successfullyCreatedTables)
        {
            foreach (var tableInfo in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() > 0))
            {
                if (TableInfoHasMissingRefTables(successfullyCreatedTables, tableInfo) == false)
                {
                    //Create the table
                    CreateTable(sqlCommand, tableInfo.Value.TableName, tableInfo.Value.GetCreateStatement(),
                                successfullyCreatedTables);
                }
            }
            if (successfullyCreatedTables.Count < _metaInfoStore.TableInfos.Count())
            {
                CreateTablesWithReferences(sqlCommand, successfullyCreatedTables);
            }
        }

        private void CreateTable(SqlCommand sqlCommand, string tableName, string sqlStatement,
                                 List<string> successfullyCreatedTables)
        {
            IEnumerable<string> sqlStatementBatches = GetBatchesFromSqlStatement(sqlStatement);
            foreach (string sql in sqlStatementBatches)
            {
                sqlCommand.CommandText = sql;
                sqlCommand.ExecuteNonQuery();
            }
            successfullyCreatedTables.Add(tableName);
        }

        private bool TableInfoHasMissingRefTables(List<string> successfullyCreatedTables,
                                                  KeyValuePair<Type, TableInfo> tableInfo)
        {
            foreach (ReferenceInfo columnInfo in tableInfo.Value.References)
            {
                if (successfullyCreatedTables.Where(x => x == columnInfo.ReferenceType.Name).Count() == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}