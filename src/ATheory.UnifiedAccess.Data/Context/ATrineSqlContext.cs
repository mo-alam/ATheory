/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.UnifiedAccess.Data.Infrastructure;

namespace ATheory.UnifiedAccess.Data.Context
{
    /// <summary>
    /// DbContext for SQL Server/SQL Express/SQLLite/MySQL
    /// </summary>
    public class ATrineSqlContext : UnifiedContext
    {
        public ATrineSqlContext(Connection conn) : base(conn)
        {
        }
    }
}