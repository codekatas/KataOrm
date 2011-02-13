using System.Data.SqlClient;
using KataOrm.MetaStore;

namespace KataOrm.Crud
{
    public abstract class DatabaseAction
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _sqlTransaction;
        protected MetaInfoStore MetaInfoStore { get; private set; }

        protected DatabaseAction(SqlConnection connection, SqlTransaction sqlTransaction, MetaInfoStore metaInfoStore)
        {
            _connection = connection;
            _sqlTransaction = sqlTransaction;
            MetaInfoStore = metaInfoStore;
        }

        protected SqlCommand CreateSqlCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _sqlTransaction;
            return command;
        }
    }
}