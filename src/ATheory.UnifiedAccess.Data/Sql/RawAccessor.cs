/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Data.Common;

namespace ATheory.UnifiedAccess.Data.Sql
{
    internal class RawAccessor : IRawAccessor
    {
        #region Constructor

        internal RawAccessor(DbConnection conn) => connection = conn;

        #endregion

        #region Private members

        DbConnection connection;

        #endregion

        #region Implements interface

        object IRawAccessor.ExecuteReader(string sql, Func<DbDataReader, object> readerAction)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            return readerAction(reader);
        }

        object IRawAccessor.ExecuteScalar(string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            return command.ExecuteScalar();
        }

        #endregion
    }
}