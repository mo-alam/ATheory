/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class ExprQuerySchemaExt
    {
        #region Public methods
        /// <summary>
        /// Creates table (sql) or schema (non-sql) based on the entity
        /// </summary>
        /// <typeparam name="TSource">Type of entity</typeparam>
        /// <param name="_">ISchemaQuery pseudo instance</param>
        /// <returns>Success or failure</returns>
        public static bool CreateSchema<TSource>(this ISchemaQuery<TSource> _) 
            where TSource : class, new() => 
            ExpressionQueryExtension.ExecFunction(c => c.CreateSchema<TSource>());

        /// <summary>
        /// Deletes table (sql) or schema (non-sql) based on the entity
        /// </summary>
        /// <typeparam name="TSource">Type of entity</typeparam>
        /// <param name="_">ISchemaQuery pseudo instance</param>
        /// <returns>Success or failure</returns>
        public static bool DeleteSchema<TSource>(this ISchemaQuery<TSource> _)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(c => c.DeleteSchema<TSource>());

        /// <summary>
        /// Updates the schema (add/delete) column/attribute
        /// </summary>
        /// <typeparam name="TSource">Type of entity</typeparam>
        /// <param name="_">ISchemaQuery pseudo instance</param>
        /// <returns>Success or failure</returns>
        public static bool UpdateSchema<TSource>(this ISchemaQuery<TSource> _)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(c => c.UpdateSchema<TSource>());

        #endregion
    }
}
