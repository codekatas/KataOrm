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
            AffectedTables = new List<string>();
            _assembly = assembly;
            _metaInfoStore = new MetaInfoStore();
            _configurationSettings = configurationSettings;
        }

        public List<string> AffectedTables { get; set; }

        #region IKataSchemaManager Members

        public void CreateSchema()
        {
            AffectedTables   = new List<string>();
            _metaInfoStore.BuildMetaInfoForAssembly(_assembly);
            AffectedTables = new List<string>();
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
                                    AffectedTables);
                    }

                    CreateTablesWithReferences(sqlCommand, AffectedTables);
                }
                connection.Close();
            }
        }

        public void DeleteSchema()
        {
            AffectedTables = new List<string>();
            using (var connection = new SqlConnection(_configurationSettings.ConnectionSettings))
            {
                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = connection;
                    sqlCommand.Connection.Open();

                    foreach (var keyValuePair in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() > 0))
                    {
                        DeleteTable(sqlCommand, keyValuePair.Value.TableName, keyValuePair.Value.GetDropStatement(),
                                    AffectedTables);
                    }
                    foreach (var keyValuePair in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() == 0))
                    {
                        DeleteTable(sqlCommand, keyValuePair.Value.TableName, keyValuePair.Value.GetDropStatement(),
                                    AffectedTables);
                    }
                }
            }
        }

        private void DeleteTable(SqlCommand sqlCommand, string tableName, string deleteStatement, List<string> successfullyCreatedTables)
        {
            sqlCommand.CommandText = deleteStatement;
            sqlCommand.ExecuteNonQuery();
            successfullyCreatedTables.Remove(tableName);
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


        private void DropTablesWithReferences(SqlCommand sqlCommand, List<string> affectedTables)
        {
            foreach (var tableInfo in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() > 0))
            {
                if (TableInfoHasMissingRefTables(affectedTables, tableInfo) == false)
                {
                    DeleteTable(sqlCommand, tableInfo.Value.TableName, tableInfo.Value.GetCreateStatement(),
                                affectedTables);
                }
            }
            if (affectedTables.Count < _metaInfoStore.TableInfos.Count())
            {
                DropTablesWithReferences(sqlCommand, affectedTables);
            }
        }

        private void CreateTablesWithReferences(SqlCommand sqlCommand, List<string> affectedTables)
        {
            foreach (var tableInfo in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() > 0))
            {
                if (TableInfoHasMissingRefTables(affectedTables, tableInfo) == false)
                {
                    //Create the table
                    CreateTable(sqlCommand, tableInfo.Value.TableName, tableInfo.Value.GetCreateStatement(),
                                affectedTables);
                }
            }
            if (affectedTables.Count < _metaInfoStore.TableInfos.Count())
            {
                CreateTablesWithReferences(sqlCommand, affectedTables);
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