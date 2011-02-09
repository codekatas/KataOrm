using System;
using System.Data;
using System.Reflection;
using KataOrm.Converter;

namespace KataOrm.MetaStore
{
    public class ColumnInfo : MetaInfo
    {
        public ColumnInfo(MetaInfoStore metaInfoStore, string columnName, Type dotNetType, PropertyInfo propertyInfo)
            : this(metaInfoStore, columnName, dotNetType, DbTypeConverter.ToDbType(dotNetType), propertyInfo)
        {
        }

        public ColumnInfo(MetaInfoStore metaInfoStore, string columnName, Type dotNetType, DbType dbType,
                          PropertyInfo propertyInfo) : base(metaInfoStore)
        {
            Name = columnName;
            DotNetType = dotNetType;
            DbType = dbType;
            PropertyInfo = propertyInfo;
        }

        public Type DotNetType { get; private set; }
        public DbType DbType { get; private set; }
        protected PropertyInfo PropertyInfo { get; private set; }
        public string Name { get; private set; }
    }
}