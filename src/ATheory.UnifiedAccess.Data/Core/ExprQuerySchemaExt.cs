/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class ExprQuerySchemaExt
    {
        #region Public methods

        public static bool CreateSchema<TSource>(this ISchemaQuery<TSource> _) 
            where TSource : class, new() => 
            ExpressionQueryExtension.ExecFunction(c => c.CreateSchema<TSource>());

        public static bool DeleteSchema<TSource>(this ISchemaQuery<TSource> _)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(c => c.DeleteSchema<TSource>());

        public static bool UpdateSchema<TSource>(this ISchemaQuery<TSource> _)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(c => c.UpdateSchema<TSource>());

        #endregion
    }
}
