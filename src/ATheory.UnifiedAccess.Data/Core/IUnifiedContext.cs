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
        /// <summary>
        /// Creates or Gets the IQueryable<T> instance from the underlying provider
        /// </summary>
        /// <typeparam name="TEntity">The type of entity for which the IQueryable should be returned</typeparam>
        /// <returns>An IQueryable for the given entity type</returns>
        IQueryable<TEntity> EntitySet<TEntity>() where TEntity : class;

        /// <summary>
        /// The instance of the entity that will be inserted into the database. 
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">Entity that will be inserted.</param>
        /// <returns>Success or failure</returns>
        bool Insert<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// The instance of the entity that will be updated.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">Entity that will be updated</param>
        /// <param name="properties">Array of properties that would be updated, if nnone if provided the whole entity will be updated</param>
        /// <returns>Success or failure</returns>
        bool Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] properties) where TEntity : class;

        /// <summary>
        /// Updates the entity. Not to be used for MongoDB
        /// </summary>
        /// <param name="predicate">A function for a condition to update elements.</param>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>Success or failure</returns>
        bool Update<TEntity>(Expression<Func<TEntity, bool>> predicate, TEntity entity) where TEntity : class;

        /// <summary>
        /// Deletes the entity from the database
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>Success or failure</returns>
        bool Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Delete the entity(s) from the database
        /// </summary>
        /// <param name="predicate">A function for a condition to delete elements.</param>
        /// <returns>Success or failure</returns>
        bool Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        /// <summary>
        /// Inserts in bulk, the entire data table
        /// </summary>
        /// <param name="entities">List of TSource elements that'll be inserted in to the table</param>
        /// <returns>Success or failure</returns>
        bool InsertBulk<TEntity>(IList<TEntity> entities) where TEntity : class;

        /// <summary>
        /// Creates a new schema (table) in to the database
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Success or failure</returns>
        bool CreateSchema<TEntity>() where TEntity : class;

        /// <summary>
        /// Alters/modifies the schema (table)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Success or failure</returns>
        bool UpdateSchema<TEntity>() where TEntity : class;

        /// <summary>
        /// Deletes the schema (table)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Success or failure</returns>
        bool DeleteSchema<TEntity>() where TEntity : class;
    }
}