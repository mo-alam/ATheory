/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using static System.String;

namespace ATheory.UnifiedAccess.Data.Sql
{
    public static class QueryParseHelper
    {
        public static string InsertTopLogicIfNeeded(this string _)
        {
            if (_.IndexOf("top", System.StringComparison.OrdinalIgnoreCase) > 0) return _;
            return _.Insert(_.IndexOf("select", System.StringComparison.OrdinalIgnoreCase) + 6, " top(1) ");
        }

        public static string InsertLastLogicIfNeeded(this string _)
        {
            if (_.IndexOf("order by", System.StringComparison.OrdinalIgnoreCase) > 0) return _;
            return _.Insert(_.IndexOf("select", System.StringComparison.OrdinalIgnoreCase) + 6, " row_number() over (order by (select null)) as arns_row_index_ident, ") +
                " order by arns_row_index_ident desc";
        }
    }
}
