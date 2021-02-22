/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATheory.UnifiedAccess.Data.Core
{
    public interface IContextAsync : IUnifiedContext
    {
        Task<TEntity> GetAsync<TEntity>(object hashKey) where TEntity : class;
        Task InsertOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteAsync<TEntity>(object hashKey) where TEntity : class;
        Task InsertBulkAsync<TEntity>(IList<TEntity> entities) where TEntity : class;
    }
}