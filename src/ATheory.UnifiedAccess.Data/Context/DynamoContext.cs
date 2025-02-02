﻿/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Helper;
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.UnifiedAccess.Data.Internal;
using ATheory.UnifiedAccess.Data.Providers;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Context
{
    public class DynamoContext : IContextAsync, ISingleLife
    {
        #region Constructor
        
        public DynamoContext(Connection conn)
        {
            database = new AmazonDynamoDBClient(conn.Key, conn.EndPoint, conn.UserDefined.GetAmazonRegion());
            context = new DynamoDBContext(database);
        }

        #endregion

        #region Private members

        readonly IAmazonDynamoDB database;
        readonly DynamoDBContext context;

        #endregion

        #region Private methods

        string GetName<TEntity>() => GetRegisteredTypes()[typeof(TEntity)].container;
        bool ExecAuxilary<TEntity>(Func<DynamoAuxiliary, (string container, KeyTypeStore keyStore), bool> func)
        {
            var auxilary = new DynamoAuxiliary(database);
            var store = GetRegisteredTypes()[typeof(TEntity)];
            return func(auxilary, store);
        }

        #endregion

        #region Implement ISingleLife

        void ISingleLife.Shutdown()
        {
            database?.Dispose();
            context?.Dispose();
        }

        #endregion

        #region Implement interface IUnifiedContext

        public IQueryable<TEntity> EntitySet<TEntity>() where TEntity : class
        {
            var provider = new DynamoQueryProvider(database);
            return provider.CreateQuery<TEntity>(GetName<TEntity>());
        }

        public bool Insert<TEntity>(TEntity entity) where TEntity : class 
            => ExecAuxilary<TEntity>((a, b) => a.InsertItem(entity, b.container));

        //Note: pass the properties that are new for this entity
        public bool Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] properties) where TEntity : class 
            => ExecAuxilary<TEntity>((a, b) => a.UpdateItem(entity, b.container, b.keyStore, properties));

        public bool Update<TEntity>(Expression<Func<TEntity, bool>> predicate, TEntity entity) where TEntity : class 
            => ExecAuxilary<TEntity>((a, b) => a.UpdateItem(entity, b.container, b.keyStore, predicate));

        public bool Delete<TEntity>(TEntity entity) where TEntity : class 
            => ExecAuxilary<TEntity>((a, b) => a.DeleteItem(entity, b.container, b.keyStore));

        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class 
            => ExecAuxilary<TEntity>((a, b) => a.DeleteItem(b.container, predicate));

        public bool InsertBulk<TEntity>(IList<TEntity> entities) where TEntity : class
            => ExecAuxilary<TEntity>((a, b) => a.InsertBulk(b.container, entities));

        // Infrastructure implementation; nothing to dispose
        public void Dispose() { }

        public bool CreateSchema<TEntity>() where TEntity : class
            => new DynamoAuxiliary(database).CreateSchema<TEntity>();

        public bool UpdateSchema<TEntity>() where TEntity : class
            => new DynamoAuxiliary(database).UpdateSchema<TEntity>();

        public bool DeleteSchema<TEntity>() where TEntity : class
            => new DynamoAuxiliary(database).DeleteSchema<TEntity>();

        #endregion

        #region Async methods (using the context)

        /// <summary>
        /// Gets an entity matching the partition key
        /// </summary>
        /// <typeparam name="TEntity">Entity type decorated with Amazon class and property attributes</typeparam>
        /// <param name="hashKey">Partition key</param>
        /// <returns>Instance of TEntity</returns>
        public async Task<TEntity> GetAsync<TEntity>(object hashKey) where TEntity : class
            => await context.LoadAsync<TEntity>(hashKey);

        /// <summary>
        /// Adds a new entity in the collection. This method assumes that the entity will be decorated with Amazon class and property attributes.
        /// </summary>
        /// <typeparam name="TEntity">Entity type decorated with Amazon class and property attributes</typeparam>
        /// <param name="entity">Object to add</param>
        /// <returns>Task</returns>
        public async Task InsertOrUpdateAsync<TEntity>(TEntity entity) where TEntity : class 
            => await context.SaveAsync(entity);

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type decorated with Amazon class and property attributes</typeparam>
        /// <param name="hashKey">Partition key</param>
        /// <returns>Task</returns>
        public async Task DeleteAsync<TEntity>(object hashKey) where TEntity : class
            => await context.DeleteAsync<TEntity>(hashKey);

        /// <summary>
        /// Inserts multiple items in the collection
        /// </summary>
        /// <typeparam name="TEntity">Entity type decorated with Amazon class and property attributes</typeparam>
        /// <param name="entities">Items to be inserted</param>
        /// <returns>Task</returns>
        public async Task InsertBulkAsync<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            var batchWrite = context.CreateBatchWrite<TEntity>();
            batchWrite.AddPutItems(entities);
            await batchWrite.ExecuteAsync();
        }

        #endregion
    }
}
