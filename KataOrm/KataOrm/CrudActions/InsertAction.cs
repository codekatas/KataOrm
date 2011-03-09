using System;
using System.Data.SqlClient;
using KataOrm.MetaStore;

namespace KataOrm.CrudActions
{
    public class InsertAction : DatabaseAction, IInsertAction
    {
        public InsertAction(SqlConnection connection, SqlTransaction sqlTransaction, MetaInfoStore metaInfoStore) : base(connection, sqlTransaction, metaInfoStore)
        {
        }

        public TEntity Insert<TEntity>(TEntity entity)
        {
            using (var command = CreateSqlCommand())
            {
                var tableInfo = MetaInfoStore.GetTableInfoFor<TEntity>();
                command.CommandText = tableInfo.GetInsertStatement();


            }
            throw new MethodAccessException();
        }
    }
}