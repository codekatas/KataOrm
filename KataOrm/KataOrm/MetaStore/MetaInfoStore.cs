using System;
using System.Collections.Generic;
using System.Reflection;
using KataOrm.Attributes;

namespace KataOrm.MetaStore
{
    public class MetaInfoStore
    {
        public Dictionary<Type, TableInfo> TableInfos = new Dictionary<Type, TableInfo>();

        public void BuildMetaInfoForAssembly(Assembly assembly)
        {
            LoadTableInfosForAssembley(assembly);
        }

        private void LoadTableInfosForAssembley(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof (TableAttribute));
                if (attributes.Length > 0)
                {
                    var attribute = (TableAttribute) attributes[0];
                    var tableInfo = new TableInfo(this, attribute.TableName, type);
                    TableInfos.Add(type, tableInfo);
                }
            }

            foreach (var tableInfo in TableInfos)
            {
                LoopThroughItsPropertiesWith<PrimaryKeyAttribute>(tableInfo.Key, tableInfo.Value,
                                                                  Action_AddPrimaryKeyInfo);
            }

            foreach (var tableInfo in TableInfos)
            {
                LoopThroughItsPropertiesWith<ColumnAttribute>(tableInfo.Key, tableInfo.Value, Action_AddColumnInfo);
                LoopThroughItsPropertiesWith<ReferenceColumnAttribute>(tableInfo.Key, tableInfo.Value,
                                                                       Action_AddReference);
            }
        }

        private void Action_AddReference(TableInfo tableInfo, PropertyInfo propertyInfo,
                                         ReferenceColumnAttribute referenceColumnAttribute)
        {
            var referenceKey = new ReferenceInfo(this, referenceColumnAttribute.ReferenceColumnName,
                                                 propertyInfo.PropertyType, propertyInfo);
            tableInfo.AddReferenceKey(referenceKey);
        }

        private void Action_AddPrimaryKeyInfo(TableInfo tableInfo, PropertyInfo propertyInfo,
                                              PrimaryKeyAttribute primaryKeyAttribute)
        {
            var columnInfo = new ColumnInfo(this, primaryKeyAttribute.PrimaryKeyName, propertyInfo.PropertyType,
                                            propertyInfo);
            tableInfo.AddPrimaryKey(columnInfo);
        }

        private void Action_AddColumnInfo(TableInfo tableInfo, PropertyInfo propertyInfo,
                                          ColumnAttribute columnAttribute)
        {
            var columnInfo = new ColumnInfo(this, columnAttribute.ColumnName, propertyInfo.PropertyType, propertyInfo);
            tableInfo.AddColumn(columnInfo);
        }

        private void LoopThroughItsPropertiesWith<T>(Type key, TableInfo value,
                                                     Action<TableInfo, PropertyInfo, T> actionAddColumnInfo)
            where T : Attribute
        {
            foreach (PropertyInfo propertyInfo in key.GetProperties())
            {
                var attribute = GetAttribute<T>(propertyInfo);
                if (attribute != null)
                {
                    actionAddColumnInfo(value, propertyInfo, attribute);
                }
            }
        }

        public T GetAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(propertyInfo, typeof (T));
            if (attributes.Length == 0) return null;
            return attributes[0] as T;
        }

        public TableInfo GetTableInfoFor<T>()
        {
            return GetTableInfoFor(typeof (T));
        }

        public TableInfo GetTableInfoFor(Type referenceType)
        {
            if (!TableInfos.ContainsKey(referenceType))
            {
                return null;
            }
            return TableInfos[referenceType];
        }
    }
}