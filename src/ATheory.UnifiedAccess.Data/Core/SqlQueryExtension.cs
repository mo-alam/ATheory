/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System.Collections.Generic;
using System.Data;
using ATheory.UnifiedAccess.Data.Sql;

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class SqlQueryExtension
    {
        /// <summary>
        /// Filters a sequence of TSource elements based on the sql statement, otherwise all.
        /// </summary>
        /// <typeparam name="TSource">Type of the select object</typeparam>
        /// <param name="sql">Select statement</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetList<TSource>(
            this ISqlQuery _,
            string sql, 
            params object[] parameters) => QueryExtension.ExecuteSql<TSource>(sql, parameters);

        /// <summary>
        /// Fetches the first record in the sequence
        /// </summary>
        /// <typeparam name="TSource">Type of the select object</typeparam>
        /// <param name="sql">Select statement</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>TSource instance</returns>
        public static TSource GetFirst<TSource>(
            this ISqlQuery _, 
            string sql, 
            params object[] parameters) => QueryExtension.ExecuteSqlSingle<TSource>(sql, false, parameters);

        /// <summary>
        /// Fetches the last record in the sequence
        /// </summary>
        /// <typeparam name="TSource">Type of the select object</typeparam>
        /// <param name="sql">Select statement</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>TSource instance</returns>
        public static TSource GetLast<TSource>(
            this ISqlQuery _, 
            string sql, 
            params object[] parameters) => QueryExtension.ExecuteSqlSingle<TSource>(sql, true, parameters);

        /// <summary>
        /// Non-Select queries: Insert/Update/Delete
        /// </summary>
        /// <param name="sql">Select statement</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Success or failure</returns>
        public static bool Execute(
            this ISqlQuery _, 
            string sql, params object[] parameters) => QueryExtension.Execute(sql, parameters);

        /// <summary>
        /// Creates the DataTable instance that would be used to populate with data for bulk insertion. InsertBulk(dataTable) method
        /// </summary>
        /// <param name="tableName">Name of table, schema must be included</param>
        /// <returns>Instance of the DataTable</returns>
        public static DataTable GetTableForBulkInsertion(
            this ISqlQuery _, 
            string tableName) => new TableSchema().GetInsertionTable(tableName);

        /// <summary>
        /// Bulk inserts in to the table
        /// </summary>
        /// <param name="dataTable">Datatable</param>
        /// <returns>Success or failure</returns>
        public static bool InsertBulk(
            this ISqlQuery _, 
            DataTable dataTable) => QueryExtension.InsertBulk(dataTable);
    }
}