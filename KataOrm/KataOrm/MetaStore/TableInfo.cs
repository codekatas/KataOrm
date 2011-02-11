﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KataOrm.MetaStore
{
    public class TableInfo : MetaInfo
    {
        private readonly Dictionary<string, ColumnInfo> _columns = new Dictionary<string, ColumnInfo>();
        private readonly  Dictionary<string, ReferenceInfo> _references =new Dictionary<string, ReferenceInfo>();

        public TableInfo(MetaInfoStore metaInfoStore, string tableName, Type type) : base(metaInfoStore)
        {
            TableName = tableName;
            EntityType = type;
        }

        public string TableName { get; private set; }
        public  Type EntityType { get; set; }
        public ColumnInfo PrimaryKey { get; set; }

        public IEnumerable<ColumnInfo> ColumnInfos
        {
            get { return _columns.Values; }
        }

        public IEnumerable<ColumnInfo> References
        {
            get { return _references.Values; }
        }


        public void AddColumn(ColumnInfo columnInfo)
        {
            if(_columns.ContainsKey(columnInfo.Name))
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
            if(_references.ContainsKey(referenceKey.Name))
            {
                throw new InvalidOperationException(string.Format("Column {0} has already been added", referenceKey.Name));        

            }
            _references.Add(referenceKey.Name, referenceKey);
        }

        public string GetSelectTableForAllFields()
        {
            StringBuilder selectBuilder = new StringBuilder("SELECT " + Escape(PrimaryKey.Name + ", "));
            AddReferenceColumns(selectBuilder);
            AddRegularColumns(selectBuilder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(selectBuilder);
            selectBuilder.Append(" FROM " + Escape(TableName));
            return selectBuilder.ToString();
        }

        private void RemoveLastCommaAndSpaceIfThereAreAnyColumns(StringBuilder selectBuilder)
        {
            if((ColumnInfos.Count() + References.Count() )> 0)
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
            foreach (var columnInfo in References)
            {
                selectBuilder.Append(Escape(columnInfo.Name) + ", ");
            }
        }

        private void AddRegularColumns(StringBuilder selectBuilder)
        {
            foreach (var columnInfo in ColumnInfos)
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
            StringBuilder insertStatementBuilder = new StringBuilder();
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
            foreach (var reference in References)
            {
                insertStatementBuilder.Append("@" + reference.Name + ", ");
            }
        }

        private void AddRegularColumnParameterNames(StringBuilder insertStatementBuilder)
        {
            foreach (var columnInfo in ColumnInfos)
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
            foreach (var columnInfo in ColumnInfos)
            {
                updateStatement.Append(Escape(columnInfo.Name) + " = @" + columnInfo.Name + ", ");
            }
        }

        private void AddReferenceColumnsNamesWithParameterName(StringBuilder updateStatement)
        {
            foreach (var reference in References)
            {
                updateStatement.Append(Escape(reference.Name) + " = @" + reference.Name + ", ");
            } 
        }
    }
}