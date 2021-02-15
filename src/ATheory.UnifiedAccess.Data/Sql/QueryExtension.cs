/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

using ATheory.UnifiedAccess.Data.Internal;
using static ATheory.Util.Reflect.Reflector;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;
using ATheory.UnifiedAccess.Data.Core;
using ATheory.Util.Extensions;

namespace ATheory.UnifiedAccess.Data.Sql
{
    internal static class QueryExtension
    {
        #region Private methods

        static List<TEntity> CreateInstances<TEntity>(DbDataReader dataReader)            
        {
            var properties = GetPropertyInfo<TEntity>();
            var result = new List<TEntity>();
            while (dataReader.Read())
            {
                var entity = SetPropertyValues<TEntity>(properties, dataReader);
                result.Add(entity);
            }
            return result;
        }

        static List<TEntity> ReadList<TEntity>(DbDataReader dataReader)
        {
            var result = new List<TEntity>();
            var type = typeof(TEntity).UnderlyingSystemType;
            while (dataReader.Read())
            {
                var value = dataReader.GetValue(0);
                var entity = value != DBNull.Value
                    ? (TEntity)Convert.ChangeType(value, type)
                    : default;

                result.Add(entity);
            }
            return result;
        }
        
        static TEntity ReadValue<TEntity>(object value)
        {
            var type = typeof(TEntity).UnderlyingSystemType;
            return value != DBNull.Value
                ? (TEntity)Convert.ChangeType(value, type)
                : default;
        }

        static TEntity CreateInstance<TEntity>(object instance)
        {
            var properties = GetPropertyInfo<TEntity>();
            var entity = SetPropertyValue<TEntity>(properties, instance);
            return entity;
        }

        static List<TEntity> ExecuteRawSql<TEntity>(
            string sql,
            SqlModification modification,
            params object[] parameters)
        {
            Error.Clear();
            if (sql.IsEmpty()) return null;
            
            try
            {
                using var _context = GetContext();
                var dependencies = _context.GetRDFDependencies();
                using var critial = dependencies.CriticalSection;
                if (modification == SqlModification.Top || modification == SqlModification.Bottom)
                {
                    sql = (modification == SqlModification.Bottom ? sql.InsertLastLogicIfNeeded() : sql).InsertTopLogicIfNeeded();
                }
                var sqlCommand = dependencies.CommandBuilder.Build(sql, parameters);

                using var reader = sqlCommand
                    .RelationalCommand
                    .ExecuteReader(
                        _context.CreateCommandParameterObject(dependencies, sqlCommand.ParameterValues)
                    )?.DbDataReader;

                return typeof(TEntity).IsValueType
                    ? ReadList<TEntity>(reader)
                    : CreateInstances<TEntity>(reader);
            }
            catch (Exception e)
            {
                Error.Set(e, ErrorOrigin.SqlRawSource);
                return null;
            }
        }
        
        static void FillRow<TEntity>(DataRow row, TEntity entity, Dictionary<string, PropertyInfo> propColInfo, ColumnSchema autoCol)
        {
            foreach (var propInfo in propColInfo)
            {
                if (string.Equals(propInfo.Key, autoCol.Name, StringComparison.OrdinalIgnoreCase)) continue;
                var prop = propInfo.Value;
                var value = prop.GetValue(entity);
                var is_sortof_ref = !prop.PropertyType.IsValueType || prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) != null;
                if (is_sortof_ref)
                {
                    if (value != null) row[propInfo.Key] = value;
                } else
                    row[propInfo.Key] = value;
            }
        }

        #endregion

        #region Internal Methods

        internal static List<TEntity> ExecuteSql<TEntity>(
            string sql,
            params object[] parameters)
        {
            return ExecuteRawSql<TEntity>(sql, SqlModification.None, parameters);
        }

        internal static TEntity ExecuteSqlSingle<TEntity>(
            string sql,
            bool isLast,
            params object[] parameters)
        {
            return ExecuteRawSql<TEntity>(sql, isLast ? SqlModification.Bottom : SqlModification.Top, parameters).FirstOrDefault();
        }

        //Insert/Update/Delete
        internal static bool Execute(string sql, params object[] parameters)
        {
            Error.Clear();
            if (sql.IsEmpty()) return false;

            try
            {
                using var _context = GetContext();
                _context.GetDbFacade().ExecuteSqlRaw(sql, parameters);
                return true;
            }
            catch (Exception e)
            {
                Error.Set(e, ErrorOrigin.SqlRawSource);
                return false;
            }
        }

        internal static void ExecRawSql(Action<IRawAccessor> readerAction)
        {
            using var context = GetContext();
            var connection = context.GetDbFacade().GetDbConnection();
            var needClosing = false;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                needClosing = true;
            }
            
            var accesor = new RawAccessor(connection);
            readerAction(accesor);

            if (needClosing) connection.Close();
        }

        internal static DataTable PopulateTable<TEntity>(IList<TEntity> entities)
        {
            var tableProc = new TableSchema();
            var dataTable = tableProc.GetInsertionTable<TEntity>();
            if (dataTable == null) return null;
            var propColInfo = GetPropertyColumnInfo<TEntity>();
            foreach (var entity in entities)
            {
                var row = dataTable.NewRow();
                FillRow(row, entity, propColInfo, tableProc.AutoIncrementColumn);
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        internal static bool InsertBulk(DataTable dataTable)
        {
            using var context = GetContext();
            return context.ToCore().InsertBulk(dataTable);
        }

        internal static bool Execute(IUnifiedContext context, string sql, params object[] parameters)
        {
            Error.Clear();
            if (sql.IsEmpty()) return false;

            try
            {
                context.GetDbFacade().ExecuteSqlRaw(sql, parameters);
                return true;
            }
            catch (Exception e)
            {
                Error.Set(e, ErrorOrigin.SqlRawSource);
                return false;
            }
        }

        #endregion

        #region Internal enum/constants

        enum SqlModification
        {
            None,
            Top,
            Bottom
        }

        #endregion
    }
}