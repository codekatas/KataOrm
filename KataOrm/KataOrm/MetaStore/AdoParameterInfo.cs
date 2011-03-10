using System.Data;

namespace KataOrm.MetaStore
{
    public class AdoParameterInfo
    {
        public string Name { get; private set; }
        public DbType DbType { get; private set; }
        public object Value { get; private set; }

        public AdoParameterInfo(string name, DbType dbType, object value)
        {
            Name = name;
            DbType = dbType;
            Value = value;
        }
    }
}