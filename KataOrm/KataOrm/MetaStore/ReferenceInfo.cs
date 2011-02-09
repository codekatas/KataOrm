using System;
using System.Reflection;

namespace KataOrm.MetaStore
{
    public class ReferenceInfo : ColumnInfo
    {
        public ReferenceInfo(MetaInfoStore metaInfoStore, string name, Type referenceType, PropertyInfo propertyInfo)
            : base(metaInfoStore, name,
                   metaInfoStore.GetTableInfoFor(referenceType).PrimaryKey.DotNetType,
                   metaInfoStore.GetTableInfoFor(referenceType).PrimaryKey.DbType, propertyInfo)
        {
            ReferenceType = referenceType;
        }

        public Type ReferenceType { get; private set; }
    }
}