using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using KataOrm.Attributes;
using KataOrm.Infrastructure;

namespace KataOrm.MetaStore
{
    public class TableInfo : MetaInfo
    {
        private readonly Dictionary<string, ColumnInfo> _columns = new Dictionary<string, ColumnInfo>();
        private readonly Dictionary<string, ReferenceInfo> _references = new Dictionary<string, ReferenceInfo>();

        public TableInfo(MetaInfoStore metaInfoStore, string tableName, Type type) : base(metaInfoStore)
        {
            TableName = tableName;
            EntityType = type;
        }

        public string TableName { get; private set; }
        public Type EntityType { get; set; }
        public ColumnInfo PrimaryKey { get; set; }

        public IEnumerable<ColumnInfo> ColumnInfos
        {
            get { return _columns.Values; }
        }

        public IEnumerable<ReferenceInfo> References
        {
            get { return _references.Values; }
        }

        public void AddColumn(ColumnInfo columnInfo)
        {
            if (_columns.ContainsKey(columnInfo.Name))
            {
                throw new InvalidOperationException(string.Format("Column {0} has already been added", columnInfo.Name));
            }
            _columns.Add(columnInfo.Name, columnInfo);
        }

        public void AddPrimaryKey(ColumnInfo primaryKey)
        {
            PrimaryKey = primaryKey;
        }

        public void AddReferenceKey(ReferenceInfo referenceKey)
        {
            if (_references.ContainsKey(referenceKey.Name))
            {
                throw new InvalidOperationException(string.Format("Column {0} has already been added", referenceKey.Name));
            }
            _references.Add(referenceKey.Name, referenceKey);
        }

        public string GetSelectTableForAllFields()
        {
            var selectBuilder = new StringBuilder("SELECT " + Escape(PrimaryKey.Name + ", "));
            AddReferenceColumns(selectBuilder);
            AddRegularColumns(selectBuilder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(selectBuilder);
            selectBuilder.Append(" FROM " + Escape(TableName));
            return selectBuilder.ToString();
        }

        private void RemoveLastCommaAndSpaceIfThereAreAnyColumns(StringBuilder selectBuilder)
        {
            if ((ColumnInfos.Count() + References.Count()) > 0)
            {
                RemoveLastCharacters(selectBuilder, 2);
            }
        }

        private void RemoveLastCharacters(StringBuilder stringBuilder, int numberOfCharacters)
        {
            stringBuilder.Remove(stringBuilder.Length - numberOfCharacters, numberOfCharacters);
        }

        private void AddReferenceColumns(StringBuilder selectBuilder)
        {
            foreach (ReferenceInfo columnInfo in References)
            {
                selectBuilder.Append(Escape(columnInfo.Name) + ", ");
            }
        }

        private void AddRegularColumns(StringBuilder selectBuilder)
        {
            foreach (ColumnInfo columnInfo in ColumnInfos)
            {
                selectBuilder.Append(Escape(columnInfo.Name) + ", ");
            }
        }

        private string Escape(string name)
        {
            return "[" + name + "]";
        }

        public string GetInsertStatement()
        {
            var insertStatementBuilder = new StringBuilder();
            insertStatementBuilder.Append("INSERT INTO " + Escape(TableName) + " (");
            AddReferenceColumns(insertStatementBuilder);
            AddRegularColumns(insertStatementBuilder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(insertStatementBuilder);
            insertStatementBuilder.Append(" ) VALUES (");
            AddReferenceColumnParameterNames(insertStatementBuilder);
            AddRegularColumnParameterNames(insertStatementBuilder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(insertStatementBuilder);
            insertStatementBuilder.Append("); SELECT SCOPE_IDENTITY");
            return insertStatementBuilder.ToString();
        }

        private void AddReferenceColumnParameterNames(StringBuilder insertStatementBuilder)
        {
            foreach (ReferenceInfo reference in References)
            {
                insertStatementBuilder.Append("@" + reference.Name + ", ");
            }
        }

        private void AddRegularColumnParameterNames(StringBuilder insertStatementBuilder)
        {
            foreach (ColumnInfo columnInfo in ColumnInfos)
            {
                insertStatementBuilder.Append("@" + columnInfo.Name + ", ");
            }
        }

        public string GetUpdateStatement()
        {
            var updateStatement = new StringBuilder();
            updateStatement.Append("UPDATE " + Escape(TableName) + " SET ");
            AddReferenceColumnsNamesWithParameterName(updateStatement);
            AddRegularColumnsNamesWithParameterName(updateStatement);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(updateStatement);
            AddWhereClauseOnId(updateStatement);
            return updateStatement.ToString();
        }

        private void AddWhereClauseOnId(StringBuilder updateStatement)
        {
            updateStatement.Append(" WHERE " + Escape(PrimaryKey.Name) + " = @" + PrimaryKey.Name);
        }

        private void AddRegularColumnsNamesWithParameterName(StringBuilder updateStatement)
        {
            foreach (ColumnInfo columnInfo in ColumnInfos)
            {
                updateStatement.Append(Escape(columnInfo.Name) + " = @" + columnInfo.Name + ", ");
            }
        }

        private void AddReferenceColumnsNamesWithParameterName(StringBuilder updateStatement)
        {
            foreach (ReferenceInfo reference in References)
            {
                updateStatement.Append(Escape(reference.Name) + " = @" + reference.Name + ", ");
            }
        }

        public string GetCreateStatement()
        {
            var createStatementBuilder = new StringBuilder();
            createStatementBuilder.AppendLine("SET ANSI_NULLS ON");
            AddGoStatement(createStatementBuilder);

            createStatementBuilder.AppendLine("SET QUOTED_IDENTIFIER ON");
            AddGoStatement(createStatementBuilder);

            createStatementBuilder.AppendLine("CREATE TABLE [dbo]." + Escape(TableName) + "(");
            AddPrimaryKeyForSchemaCreate(createStatementBuilder);
            AddRegularColumnsForSchemaCreate(createStatementBuilder);
            AddForiegnKeyColumnsForSchemaCreate(createStatementBuilder);
            AddPrimaryKeyConstraint(createStatementBuilder);
            createStatementBuilder.AppendLine(" )");
            createStatementBuilder.AppendLine("ON [PRIMARY]");
            AddGoStatement(createStatementBuilder);
            AddForeignKeyConstraints(createStatementBuilder);

            Log.BoundTo(this).Log(createStatementBuilder.ToString());
            return createStatementBuilder.ToString();
        }

        private void AddForeignKeyConstraints(StringBuilder createStatementBuilder)
        {
            foreach (ReferenceInfo reference in References)
            {
                string constraintName = Escape("FK_" + TableName + "_" + reference.ReferenceType.Name);
                createStatementBuilder.AppendLine("ALTER TABLE [dbo]." + Escape(TableName) +
                                                  " WITH CHECK ADD  CONSTRAINT " +
                                                  constraintName + "FOREIGN KEY(" +
                                                  Escape(reference.Name) + ")");
                createStatementBuilder.AppendLine("REFERENCES [dbo]." + Escape(reference.ReferenceType.Name) + " (" +
                                                  Escape(PrimaryKey.Name) + ")");
                AddGoStatement(createStatementBuilder);

                createStatementBuilder.AppendLine("ALTER TABLE [dbo]." + Escape(TableName) + " CHECK CONSTRAINT " +
                                                  constraintName);
                AddGoStatement(createStatementBuilder);
            }
        }

        private void AddForiegnKeyColumnsForSchemaCreate(StringBuilder createStatementBuilder)
        {
            foreach (ReferenceInfo reference in References)
            {
                createStatementBuilder.AppendLine(Escape(reference.Name) + " " + Escape(reference.SqlDbType.ToString()) +
                                                  " NULL, ");
            }
        }

        private void AddGoStatement(StringBuilder createStatementBuilder)
        {
            createStatementBuilder.AppendLine("GO");
        }

        private void AddPrimaryKeyConstraint(StringBuilder createStatementBuilder)
        {
            createStatementBuilder.AppendLine("CONSTRAINT " + Escape("PK_" + TableName + "_" + PrimaryKey.Name) +
                                              " PRIMARY KEY CLUSTERED ");
            createStatementBuilder.AppendLine("( " + Escape(PrimaryKey.Name) + " ASC )");
            createStatementBuilder.AppendLine(
                "WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
        }

        private void AddPrimaryKeyForSchemaCreate(StringBuilder createStatementBuilder)
        {
            createStatementBuilder.AppendLine(Escape(PrimaryKey.Name) + " " + Escape(PrimaryKey.SqlDbType.ToString()) +
                                              " IDENTITY(1,1) NOT NULL, ");
        }

        private void AddRegularColumnsForSchemaCreate(StringBuilder createStatementBuilder)
        {
            foreach (ColumnInfo columnInfo in ColumnInfos)
            {
                createStatementBuilder.AppendLine(Escape(columnInfo.Name) + " " +
                                                  Escape(columnInfo.SqlDbType.ToString()) + " NULL, ");
            }
        }

        public string GetDropStatement()
        {
            return "Drop table " + TableName;
        }

        public List<AdoParameterInfo> GetParametersForInsert(object entity)
        {
            return GetParametersForReferenceAndRegularColumns(entity);
        }

        private List<AdoParameterInfo> GetParametersForReferenceAndRegularColumns(object entity)
        {
            var parameters = new List<AdoParameterInfo>();

            foreach (ReferenceInfo referenceInfo in References)
            {
                object referencedEntity = referenceInfo.PropertyInfo.GetValue(entity, null);
                PropertyInfo referencePrimaryKeyProperty =
                    MetaInfoStore.GetTableInfoFor(referenceInfo.ReferenceType).PrimaryKey.PropertyInfo;
                if (referencedEntity == null)
                {
                    parameters.Add(new AdoParameterInfo(referenceInfo.Name, referenceInfo.DbType, null));
                }
                else
                {
                    parameters.Add(new AdoParameterInfo(referenceInfo.Name, referenceInfo.DbType,
                                                        referencePrimaryKeyProperty.GetValue(referencedEntity, null)));
                }
            }

            foreach (ColumnInfo columnInfo in ColumnInfos)
            {
                MemberTypes memberType = columnInfo.PropertyInfo.MemberType;

                TestMethod(entity, columnInfo.PropertyInfo);
                object val = columnInfo.PropertyInfo.GetValue(entity, null);

                parameters.Add(new AdoParameterInfo(columnInfo.Name, columnInfo.DbType,
                                                    columnInfo.PropertyInfo.GetValue(entity, null)));
            }
            return parameters;
        }

        private void TestMethod(object o, PropertyInfo propertyInfo)
        {
            Type t = o.GetType();
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] propattr = prop.GetCustomAttributes(false);
                object attr = (from row in propattr where row.GetType() == typeof (ColumnAttribute) select row).FirstOrDefault();
                if (attr == null) continue;
                var myattr = (ColumnAttribute)attr;
                object value = prop.GetValue(o, null);
            }
        }
    }
}