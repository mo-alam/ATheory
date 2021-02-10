/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Data.Common;

namespace ATheory.UnifiedAccess.Data.Sql
{
    internal interface IRawAccessor {
        object ExecuteReader(string sql, Func<DbDataReader, object> reader);
        object ExecuteScalar(string sql);
    }
}