using System.Collections.Generic;
using System.Reflection;

namespace KataOrm.Test.Helper
{
    public interface IKataSchemaManager
    {
        void CreateSchema();
        void DeleteSchema();
        IEnumerable<string> GetBatchesFromSqlStatement(string sqlStatement);
        List<string> AffectedTables { get; set; }       
    }
}