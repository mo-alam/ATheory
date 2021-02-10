/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    /// <summary>
    /// Provider, accessor: 
    /// </summary>
    public interface IGateway
    {
        /// <summary>
        /// Registers the given entity type in the model
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="keys">Properties that are to be registered as unique key</param>
        /// <returns>The factory</returns>
        IGateway Register<TEntity>(
            params Expression<Func<TEntity, object>>[] keys);

        /// <summary>
        /// Registers the given entity type in the model
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="container">Name of the container in case of CosmosDb </param>
        /// <param name="keys">Properties that are to be registered as unique key</param>
        /// <returns>The factory</returns>
        IGateway Register<TEntity>(
            string container, 
            params Expression<Func<TEntity, object>>[] keys);

        /// <summary>
        /// Registers the given entity type in the model, use it for MongoDB, DynamoDb
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="collectionName">Collection name of this entity.</param>
        /// <param name="container">Name of the Database, if null then it will assume the default database</param>
        /// <returns>The factory</returns>
        IGateway Register<TEntity>(
            string collectionName,
            string container = null);


        /// <summary>
        /// Use this method to add special key to the entity registred in the previous Register call. 
        /// This method must be a continued call, because the entity type is temporarily cached.
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <param name="keyType">Type of key</param>
        /// <returns>The factory</returns>
        IGateway SpecialKey(
            string key,
            SpecialKey keyType = TypeCatalogue.SpecialKey.PartitionKey);
    }
}