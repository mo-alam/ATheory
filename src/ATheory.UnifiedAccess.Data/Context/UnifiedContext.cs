/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;
using System.Collections.Generic;
using ATheory.UnifiedAccess.Data.Sql;
using ATheory.Util.Extensions;

namespace ATheory.UnifiedAccess.Data.Context
{
    public class UnifiedContext : DbContext, IUnifiedContext
    {
        #region Constructor

        public UnifiedContext(Connection conn) : base()
        {
            connection = conn;
        }

        #endregion

        #region Protected members

        protected Connection connection;
        
        #endregion

        #region Protected methods

        protected virtual bool ExecFunction(Func<bool> func)
        {
            Error.Clear();
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return false;
            }
        }

        protected virtual void RegisterEntity(
            EntityTypeBuilder typeBuilder,
            (string container, KeyTypeStore keyStore) args)
        {
            if (args.keyStore.Keys != null)
                typeBuilder.HasKey(args.keyStore.Keys);
        }

        protected virtual bool CreateEntitySchema<TEntity>()
        {
            var (container, keyStore) = GetRegisteredTypes()[typeof(TEntity)];
            var scripts = new Scripts().GenerateCreateTable<TEntity>(keyStore);
            return QueryExtension.Execute(this, scripts.ToString());
        }

        protected virtual bool UpdateEntitySchema<TEntity>() where TEntity : class
        {
            var (container, keyStore) = GetRegisteredTypes()[typeof(TEntity)];
            var scripts = new Scripts().GenerateAlterTable<TEntity>(keyStore);
            var result = true;

            foreach (var script in scripts)
            {
                result &= QueryExtension.Execute(this, script);
            }
            return result;
        }

        protected virtual bool DeleteEntitySchema<TEntity>() where TEntity : class
        {
            var (container, keyStore) = GetRegisteredTypes()[typeof(TEntity)];
            var scripts = new Scripts().GenerateDropTable<TEntity>(keyStore);
            return QueryExtension.Execute(this, scripts.ToString());
        }

        #endregion

        #region Overriden methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (connection.Provider)
            {
                case StorageProvider.SqlServer:
                    optionsBuilder.UseSqlServer(connection.ConnectionString);
                    break;
                case StorageProvider.SqlLite:
                    optionsBuilder.UseSqlite(connection.ConnectionString);
                    break;
                case StorageProvider.MySql:
                    optionsBuilder.UseMySQL(connection.ConnectionString);
                    break;
                case StorageProvider.Cosmos:
                    optionsBuilder.UseCosmos(connection.EndPoint, connection.Key, databaseName: connection.Database);
                    break;
                case StorageProvider.PostgreSql:
                    break;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            GetRegisteredTypes().ForEach(pair => RegisterEntity(modelBuilder.Entity(pair.Key), pair.Value));
        }

        #endregion

        #region Implement interface

        public IQueryable<TEntity> EntitySet<TEntity>() where TEntity : class => Set<TEntity>();
        public bool Insert<TEntity>(TEntity entity) where TEntity : class
        {
            return ExecFunction(() =>
            {
                Set<TEntity>().Add(entity);
                return SaveChanges() > 0;
            });
        }

        public bool Update<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
            where TEntity : class
        {
            return ExecFunction(() =>
            {
                var entry = Entry(entity);
                if (properties.Length > 0)
                {
                    entry.State = EntityState.Unchanged;
                    properties.ForEach(p => entry.Property(p).IsModified = true);
                }
                else
                    entry.State = EntityState.Modified;
                return SaveChanges() > 0;
            });
        }

        public bool Update<TEntity>(Expression<Func<TEntity, bool>> predicate, TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class
        {
            return ExecFunction(() =>
            {
                Remove(entity);
                return SaveChanges() > 0;
            });
        }

        public bool Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var entity = Set<TEntity>().Where(predicate).ToArray();
            return entity == null || entity.Length == 0
                ? false
                : ExecFunction(() =>
                {
                    RemoveRange(entity);
                    return SaveChanges() > 0;
                });
        }

        public bool InsertBulk<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            if (entities == null || entities.Count < 1) return false;
            var dataTable = QueryExtension.PopulateTable(entities);

            return InsertBulk(dataTable);
        }

        public bool CreateSchema<TEntity>() where TEntity : class
            => ExecFunction(() => CreateEntitySchema<TEntity>());

        public bool UpdateSchema<TEntity>() where TEntity : class 
            => ExecFunction(() => UpdateEntitySchema<TEntity>());

        public bool DeleteSchema<TEntity>() where TEntity : class
            => ExecFunction(() => DeleteEntitySchema<TEntity>());

        #endregion

        #region Public members (class implementation)

        public bool InsertBulk(DataTable table)
        {
            Error.Clear();
            try
            {
                var options = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction | SqlBulkCopyOptions.KeepIdentity;
                var connection = (SqlConnection)Database.GetDbConnection();
                var needClosing = false;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    needClosing = true;
                }
                try
                {
                    using (var bulkCopy = new SqlBulkCopy(connection, options, null) { DestinationTableName = table.TableName })
                    {
                        bulkCopy.WriteToServer(table);
                    }
                    return true;
                }
                catch (Exception inner)
                {
                    Error.SetContext(inner);
                    return false;
                }
                finally
                {
                    if (needClosing) connection.Close();
                }
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return false;
            }
        }
        
        #endregion
    }
}