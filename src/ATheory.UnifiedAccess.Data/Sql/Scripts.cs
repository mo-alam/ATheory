/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using ATheory.Util.Reflect;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ATheory.UnifiedAccess.Data.Sql
{
    /// <summary>
    /// Internal class to provide sql script templates
    /// </summary>
    internal class Scripts
    {
        #region Private methods

        string ColumnScript(string name, PropertyInfo propertyInfo, KeyTypeStore keyStore, ColumnSchema schema, char comma)
        {
            var type = Type.GetTypeCode(propertyInfo.PropertyType);
            var typeStatement = dataTypes.ContainsKey(type) ? dataTypes[type] : dataTypes[TypeCode.String];
            var maxLen = Reflector.GetCustomAttribute<MaxLengthAttribute>(propertyInfo);
            var maxLenStatement = maxLen != null ? $" ({maxLen.Length})" : string.Empty;
            var required = Reflector.GetCustomAttribute<RequiredAttribute>(propertyInfo);
            var primaryKey = !keyStore.Keys.Exists(propertyInfo.Name) ? string.Empty : PrimaryKey;

            return $"[{name}] {typeStatement}{maxLenStatement}{(required == null ? string.Empty : NotNull)}{primaryKey}{comma}";
        }

        void GenerateColumnScript(StringBuilder builder, string name, PropertyInfo propertyInfo, KeyTypeStore keyStore, char comma)
            => builder.AppendLine(ColumnScript(name, propertyInfo, keyStore, null, comma));

        string GetTableName<TEntity>(KeyTypeStore keyStore)
        {
            var tableInfo = Reflector.GetTableInfo<TEntity>();
            var schema = tableInfo.schema.IsEmpty() ? string.Empty : $"{tableInfo.schema}.";
            var table = keyStore.Container.OtherIfThisEmpty(tableInfo.tableName.OtherIfThisEmpty(typeof(TEntity).Name));
            return $"{schema}{table}";
        }
        
        void GenerateDropSql(string table, List<ColumnSchema> remove, List<string> result)
        {
            if (remove.Count > 0)
            {
                var script = new StringBuilder($"alter table {table} ");
                var count = 0;
                script.AppendLine(DropColumn);
                remove.ForEach(col => script.AppendLine($"{col.Name}{((++count < remove.Count) ? ',' : ' ')}"));
                result.Add(script.ToString());
            }
        }

        void GenerateAddSql(KeyTypeStore keyStore, string table, List<(string Name, PropertyInfo Info)> add, List<string> result)
        {
            if (add.Count > 0)
            {
                var count = 0;
                var script = new StringBuilder($"alter table {table} ");
                script.AppendLine(AddColumn);
                foreach (var (Name, Info) in add)
                {
                    GenerateColumnScript(script, Name, Info, keyStore, (++count < add.Count) ? ',' : ' ');
                }
                result.Add(script.ToString());
            }
        }

        // Haven't considered PK removal
        string AlterColumnScript(string name, PropertyInfo propertyInfo, KeyTypeStore keyStore, ColumnSchema schema) 
            => $"alter column {ColumnScript(name, propertyInfo, keyStore, schema, ' ')};";

        void GenerateAlterSql(string table, Dictionary<string, (string Name, PropertyInfo Info)> colInfo, Dictionary<string, ColumnSchema> colSchemas, KeyTypeStore keyStore, List<string> result)
        {
            foreach (var col in colInfo)
            {
                if (!colSchemas.ContainsKey(col.Key)) continue;
                var info = col.Value.Info;
                if (info.PropertyType != colSchemas[col.Key].DataType)
                {
                    result.Add($"alter table {table} {AlterColumnScript(col.Value.Name, info, keyStore, colSchemas[col.Key])};");
                }
                else if (Type.GetTypeCode(info.PropertyType) == TypeCode.String)
                {
                    var maxLen = Reflector.GetCustomAttribute<MaxLengthAttribute>(info);
                    if (maxLen != null && maxLen.Length != colSchemas[col.Key].Length)
                    {
                        result.Add($"alter table {table} {AlterColumnScript(col.Value.Name, info, keyStore, colSchemas[col.Key])};");
                    }
                }
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Generates SQL scripts for Create Table.
        /// <para>1. If the column name is different than the property name, needs [Column] annotation.
        /// 2. If the string type's length is more than 255 (default), needs [MaxLength] annotation.
        /// 3. If the column is not null, needs [Required] annotation
        /// 4. Primary keys are specified at the time of entity registration.
        /// 5. If the Entity doesn't have Table data annotation and table name is different than the entity name, Keystore container needs to provide the table name.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="keyStore">Keystore as registered</param>
        /// <returns>Sql script</returns>
        internal StringBuilder GenerateCreateTable<TEntity>(KeyTypeStore keyStore)
        {
            var table = GetTableName<TEntity>(keyStore);

            var script = new StringBuilder($"create table {table} (");
            var colInfo = Reflector.GetPropertyColumnInfo<TEntity>();
            var count = 0;
            foreach (var info in colInfo) {
                GenerateColumnScript(script, info.Key, info.Value, keyStore, (++count < colInfo.Count) ? ',' : ' ');
            }
            script.AppendLine(");");
            return script;
        }

        internal List<string> GenerateAlterTable<TEntity>(KeyTypeStore keyStore)
        {
            var table = GetTableName<TEntity>(keyStore);
            var colInfo = Reflector.GetEntityColumnInfo<TEntity>();
            var colSchemas = TableSchema.GetColumnSchemas(table);
            var add = colInfo.Where(a => !colSchemas.ContainsKey(a.Key)).Select(v => v.Value).ToList();
            var remove = colSchemas.Where(a => !colInfo.ContainsKey(a.Key)).Select(v => v.Value).ToList();
            var result = new List<string>();

            GenerateDropSql(table, remove, result);
            GenerateAddSql(keyStore, table, add, result);
            GenerateAlterSql(table, colInfo, colSchemas, keyStore, result);

            return result;
        }

        internal string GenerateDropTable<TEntity>(KeyTypeStore keyStore) 
            => $"drop table {GetTableName<TEntity>(keyStore)};";

        #endregion

        #region Constants

        const string NotNull = " not null";
        const string PrimaryKey = " primary key";
        const string DropColumn = " drop column ";
        const string AddColumn = " add ";

        Dictionary<TypeCode, string> dataTypes = new Dictionary<TypeCode, string>
        {
            { TypeCode.String, "nvarchar"},
            { TypeCode.Int16, "smallint"},
            { TypeCode.Int32, "int"},
            { TypeCode.Int64, "bigint"},
            { TypeCode.Boolean, "bit"},
            { TypeCode.Decimal, "decimal"},
            { TypeCode.DateTime, "datetime"},
            { TypeCode.Single, "float"},
            { TypeCode.Double, "numeric"}
        };

        #endregion
    }
}