/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Context
{
    public class MongoContext : IUnifiedContext, ISingleLife
    {
        #region Constructor
        
        public MongoContext(Connection conn)
        {
            database = new MongoClient(conn.ConnectionString).GetDatabase(conn.Database);
        }

        #endregion

        #region Private members

        readonly IMongoDatabase database;

        #endregion

        #region Private methods

        string GetName<TEntity>() => GetRegisteredTypes()[typeof(TEntity)].container;

        TResult Exec<TEntity, TResult>(Func<IMongoCollection<TEntity>, TResult> func)
        {
            Error.Clear();
            try
            {
                return func(database.GetCollection<TEntity>(GetName<TEntity>()));
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return default;
            }
        }

        #endregion

        #region Implement interface IUnifiedContext

        public IQueryable<TEntity> EntitySet<TEntity>() where TEntity : class
        {
            Error.Clear();
            try
            {
                return database.GetCollection<TEntity>(GetName<TEntity>()).AsQueryable();
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return null;
            }
        }

        public bool Insert<TEntity>(TEntity entity) where TEntity : class =>
            Exec<TEntity, bool>(c => { c.InsertOne(entity); return true; });

        public bool Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] properties) where TEntity : class => 
            throw new NotImplementedException();

        public bool Update<TEntity>(Expression<Func<TEntity, bool>> predicate, TEntity entity) where TEntity : class =>
            Exec<TEntity, bool>(c => c.ReplaceOne(predicate, entity).IsAcknowledged);

        public bool Delete<TEntity>(TEntity entity) where TEntity : class => throw new NotImplementedException();

        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class =>
            Exec<TEntity, bool>(c => c.DeleteMany(predicate).IsAcknowledged);
        
        // Infrastructure implementation; nothing to dispose
        public void Dispose() {

        }

        public bool InsertBulk<TEntity>(IList<TEntity> entities) where TEntity : class =>
            Exec<TEntity, bool>(c => { c.InsertMany(entities); return true; });

        public bool CreateSchema<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        public bool UpdateSchema<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        public bool DeleteSchema<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implement interface ISingleLife

        void ISingleLife.Shutdown() {
            // Nothing to clean
        }

        #endregion
    }
}
