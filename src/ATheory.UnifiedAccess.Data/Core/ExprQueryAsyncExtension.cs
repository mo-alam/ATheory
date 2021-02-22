/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class ExprQueryAsyncExtension
    {
        #region Public methods

        public static Task<TSource> Get<TSource>(this IQueryAsync<TSource> _, object key)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(
                c => (c is IContextAsync asyncContext) 
                ? asyncContext.GetAsync<TSource>(key) 
                : Task.FromResult<TSource>(null));

        public static Task InsertOrUpdate<TSource>(this IQueryAsync<TSource> _, TSource source)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(
                c => (c is IContextAsync asyncContext)
                ? asyncContext.InsertOrUpdateAsync(source)
                : Task.FromException(new NotImplementedException()));

        public static Task Delete<TSource>(this IQueryAsync<TSource> _, object key)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(
                c => (c is IContextAsync asyncContext)
                ? asyncContext.DeleteAsync<TSource>(key)
                : Task.FromException(new NotImplementedException()));

        public static Task Delete<TSource>(this IQueryAsync<TSource> _, IList<TSource> sources)
            where TSource : class, new() =>
            ExpressionQueryExtension.ExecFunction(
                c => (c is IContextAsync asyncContext)
                ? asyncContext.InsertBulkAsync<TSource>(sources)
                : Task.FromException(new NotImplementedException()));

        #endregion
    }
}