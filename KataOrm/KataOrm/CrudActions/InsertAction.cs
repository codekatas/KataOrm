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
            throw new NotImplementedException();
        }
    }
}