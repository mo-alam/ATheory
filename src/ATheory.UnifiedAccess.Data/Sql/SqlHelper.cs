/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Microsoft.Data.SqlClient;

namespace ATheory.UnifiedAccess.Data.Sql
{
    public static class SqlHelper
    {
        public static class Parameters
        {
            public static SqlParameter Get(string name, string value) => string.IsNullOrWhiteSpace(value)
                ? new SqlParameter(name, typeof(string))
                : new SqlParameter(name, value);

            public static SqlParameter Get(string name, int? value) => !value.HasValue
                ? new SqlParameter(name, -1)
                : new SqlParameter(name, (object)value.Value);

            public static SqlParameter GetOfType<T>(string name, T value) => new SqlParameter(name, value);
        }
    }
}