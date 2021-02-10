/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ATheory.Util.Reflect;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Sql
{
    public class TableSchema
    {
        #region Constructor

        public TableSchema() { }
        public TableSchema(string tableName)
        {
            GetSchema(tableName);
        }

        #endregion

        #region Properties

        public List<ColumnSchema> ColumnSchemas { get; private set; }
        public string TableName { get; private set; }
        public ColumnSchema AutoIncrementColumn { get; private set; }

        #endregion

        #region Private methods

        object GetColumnSchemas(DbDataReader reader)
        {
            var table = reader.GetSchemaTable();
            if (table == null) return null;
            var result = new List<ColumnSchema>();
            object autoColumn = null;
            foreach (DataRow row in table.Rows)
            {
                var info = new ColumnSchema(row);
                result.Add(info);
                if (info.IsAutoIncrement) autoColumn = info;
            }
            ColumnSchemas = result.OrderBy(n => n.Index).ToList();
            return autoColumn;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the table schema
        /// </summary>
        /// <param name="tableName">Name of the table. Must include  schema if any</param>
        /// <returns>Success or failure</returns>
        public bool GetSchema(string tableName)
        {
            TableName = tableName;
            Error.Clear();
            try
            {
                void accessorReader(IRawAccessor accessor) {
                    if (accessor.ExecuteReader($"select * from {TableName} where 1 <> 1", GetColumnSchemas) is ColumnSchema autoColumn)
                    {
                        AutoIncrementColumn = autoColumn;
                        var seedNow = accessor.ExecuteScalar($"select max({autoColumn.Name}) from {tableName}");
                        autoColumn.IncrementSeed = (seedNow == null || seedNow == DBNull.Value) ? 1 : (long)Convert.ChangeType(seedNow, TypeCode.Int64) + 1;
                    }
                }
                
                QueryExtension.ExecRawSql(accessorReader);
                return true;
            }
            catch (Exception e)
            {
                Error.Set(e, ErrorOrigin.Schema);
                return false;
            }            
        }

        /// <summary>
        /// Gets the table schema. Model must include TableAttribute
        /// </summary>
        /// <typeparam name="TEntity">Model of the table</typeparam>
        /// <returns>Success of failure</returns>
        public bool GetSchema<TEntity>()
        {
            var tableInfo = Reflector.GetTableInfo<TEntity>();
            var schema = tableInfo.schema.IsEmptyStatement() ? string.Empty : $"{tableInfo.schema}.";
            return GetSchema($"{schema}{tableInfo.tableName}");
        }
                
        /// <summary>
        /// Creates an instance of DataTable based on the supplied column info 
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="schema">Column schemas</param>
        /// <returns>DataTable instance that can be filled with data and then pushed into the datbase</returns>
        public DataTable GetInsertionTable(string tableName, List<ColumnSchema> schema)
        {
            var table = new DataTable(tableName);
            var primaryKeys = new List<DataColumn>();
            for (int i = 0; i < schema.Count; i++)
            {
                var info = schema[i];
                var column = info.CreateDataColumn();
                if (info.IsIdentity) primaryKeys.Add(column);
                table.Columns.Add(column);
            }
            if (primaryKeys.Count > 0)
                table.PrimaryKey = primaryKeys.ToArray();
            return table;
        }

        /// <summary>
        /// Creates an instance of DataTable based on the column info processed earlier through the call GetScehma
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <returns>DataTable instance that can be filled with data and then pushed into the datbase</returns>
        public DataTable GetInsertionTable(string tableName) => GetInsertionTable(tableName, ColumnSchemas);

        /// <summary>
        /// Creates an instance of DataTable based on the column info processed earlier through the constructor
        /// </summary>
        /// <returns>DataTable instance that can be filled with data and then pushed into the datbase</returns>
        public DataTable GetInsertionTable() => GetInsertionTable(TableName);

        /// <summary>
        /// Creates an instance of DataTable based on Model T
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <returns>DataTable instance that can be filled with data and then pushed into the datbase</returns>
        public DataTable GetInsertionTable<T>() => GetSchema<T>()? GetInsertionTable(TableName) : null;
        
        #endregion
    }
}