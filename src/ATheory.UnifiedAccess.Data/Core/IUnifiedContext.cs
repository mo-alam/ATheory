/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ATheory.UnifiedAccess.Data.Core
{
    public interface IUnifiedContext : IDisposable
    {
        IQueryable<TEntity> EntitySet<TEntity>() where TEntity : class;
        bool Insert<TEntity>(TEntity entity) where TEntity : class;
        bool Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] properties) where TEntity : class;
        bool Update<TEntity>(Expression<Func<TEntity, bool>> predicate, TEntity entity) where TEntity : class;
        bool Delete<TEntity>(TEntity entity) where TEntity : class;
        bool Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        bool InsertBulk<TEntity>(IList<TEntity> entities) where TEntity : class;
    }
}