using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using KataOrm.MetaStore;

namespace KataOrm.Test.Helper
{
    public class KataSchemaManager : IKataSchemaManager
    {
        private const string ConnectionString = "";
        private MetaInfoStore _metaInfoStore;
        private Assembly _assembly;

        #region IKataSchemaManager Members

        public KataSchemaManager(Assembly assembly)
        {
            _assembly = assembly;
            _metaInfoStore = new MetaInfoStore();
        }

        public void CreateSchema()
        {
            _metaInfoStore.BuildMetaInfoForAssembly(_assembly);
            var successfullyCreatedTables = new List<string>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using(var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Connection = connection;
                    sqlCommand.Connection.Open();
                    //create tables with no reference
                    foreach (var keyValuePair in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() == 0))
                    {
                        sqlCommand.CommandText = keyValuePair.Value.GetCreateStatement();
                        sqlCommand.ExecuteNonQuery();
                        successfullyCreatedTables.Add(keyValuePair.Value.TableName);
                    }

                    foreach (var tableInfo in _metaInfoStore.TableInfos)
                    {
                    }
                }
            }

            foreach (var keyValuePair in _metaInfoStore.TableInfos.Where(x => x.Value.References.Count() > 0))
            {
                
            }
        }

        public void DeleteSchema()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}