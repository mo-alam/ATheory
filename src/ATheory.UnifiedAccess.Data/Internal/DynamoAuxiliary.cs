/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ATheory.UnifiedAccess.Data.Common;
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.UnifiedAccess.Data.Providers;
using ATheory.Util.Extensions;
using ATheory.Util.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;
using static ATheory.UnifiedAccess.Data.Internal.DynamoPartials;

namespace ATheory.UnifiedAccess.Data.Internal
{
    internal class DynamoAuxiliary
    {
        #region Constructor

        internal DynamoAuxiliary(IAmazonDynamoDB client)
        {
            database = client;
        }

        #endregion

        #region Private members

        readonly IAmazonDynamoDB database;

        #endregion

        #region Private methods

        CreateTableRequest CreateSchemaCreateRequest<TEntity>(string container, KeyTypeStore keyStore) where TEntity : class
        {
            var propInfo = Reflector.GetPropertyInfo<TEntity>();
            var attributes = new List<AttributeDefinition>();
            var keys = new List<KeySchemaElement>();
            foreach (var key in keyStore.SpecialKeys)
            {
                var name = key.Value[0];
                attributes.Add(CreateAttributeDefinition(name, propInfo[name]));
                keys.Add(CreateKeySchemaElement(key.Key, name));
            }

            return new CreateTableRequest
            {
                TableName = container,
                AttributeDefinitions = attributes,
                KeySchema = keys,
                ProvisionedThroughput = new ProvisionedThroughput(1, 1) /* Default */
            };
        }

        PutItemRequest CreateAddRequest<TEntity>(string container, TEntity entity)
        {
            var propInfo = Reflector.GetPropertyInfoValue(entity);
            return new PutItemRequest
            {
                TableName = container,
                Item = propInfo.Where(p => !p.Value.value.IsDefault()).ToDictionary(k => k.Key, v => CreateAttributeValue(v.Value.info, v.Value.value))
            };
        }

        WriteRequest CreateWriteRequest<TEntity>(TEntity entity)
        {
            var propInfo = Reflector.GetPropertyInfoValue(entity);
            return new WriteRequest
            {
                PutRequest = new PutRequest
                {
                    Item = propInfo.Where(p => !p.Value.value.IsDefault()).ToDictionary(k => k.Key, v => CreateAttributeValue(v.Value.info, v.Value.value))
                }
            };
        }

        DeleteItemRequest CreateDeleteRequest<TEntity>(string container, KeyTypeStore keyStore, TEntity entity)
        {
            var propInfo = Reflector.GetPropertyInfoValue(entity);
            var keys = keyStore.SpecialKeys.SelectMany(l => l.Value);
            return new DeleteItemRequest
            {
                TableName = container,
                Key = keys.ToDictionary(k => k, v => CreateAttributeValue(propInfo[v].info, propInfo[v].value))
            };
        }

        DeleteItemRequest CreateDeleteRequest<TEntity>(string container, Dictionary<string, VarValueTuple> members) where TEntity : class
        {
            var propInfo = Reflector.GetPropertyInfo<TEntity>();
            return new DeleteItemRequest
            {
                TableName = container,
                Key = members.ToDictionary(k => k.Key, v => CreateAttributeValue(propInfo[v.Key], v.Value.Value))
            };
        }

        Dictionary<string, AttributeValueUpdate> GetUpdates(Dictionary<string, (PropertyInfo info, object value)> propInfo, HashSet<string> keys)
        {
            Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate>();
            foreach (var info in propInfo)
            {
                if (keys.Contains(info.Key) || info.Value.value.IsDefault()) continue;
                updates.Add(info.Key, CreateAttributeValueUpdate(info.Value.info, info.Value.value));
            }
            return updates;
        }

        UpdateItemRequest CreateUpdateRequest<TEntity>(string container, KeyTypeStore keyStore, TEntity entity, Dictionary<string, VarValueTuple> members) where TEntity : class
        {
            var propInfo = Reflector.GetPropertyInfoValue(entity);
            var keys = keyStore.SpecialKeys.SelectMany(l => l.Value).ToHashSet();
            
            return new UpdateItemRequest
            {
                TableName = container,
                Key = members.ToDictionary(k => k.Key, v => CreateAttributeValue(propInfo[v.Key].info, v.Value.Value)),
                AttributeUpdates = GetUpdates(propInfo, keys)
            };
        }

        UpdateItemRequest CreateUpdateRequest<TEntity>(string container, KeyTypeStore keyStore, TEntity entity, HashSet<string> propNames)
        {
            var propInfo = Reflector.GetPropertyInfoValue(entity);
            var propInfoOnly = propInfo.Where(p => propNames.Contains(p.Key)).ToDictionary(k => k.Key, v => v.Value);
            var keys = keyStore.SpecialKeys.SelectMany(l => l.Value);

            return new UpdateItemRequest
            {
                TableName = container,
                Key = keys.ToDictionary(k => k, v => CreateAttributeValue(propInfo[v].info, propInfo[v].value)),
                AttributeUpdates = GetUpdates(propInfoOnly, keys.ToHashSet())
            };
        }

        #endregion

        #region Internal methods

        internal bool CreateSchema<TEntity>() where TEntity : class
        {
            var (container, keyStore) = GetRegisteredTypes()[typeof(TEntity)];
            if (!keyStore.HasSpecialKeys) return false;

            var request = CreateSchemaCreateRequest<TEntity>(container, keyStore);

            var result = database.CreateTableAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        // Need to think about the updated ProvisionedThroughput
        internal bool UpdateSchema<TEntity>() where TEntity : class
        {
            var result = database.UpdateTableAsync(GetName<TEntity>(), new ProvisionedThroughput(2, 2));
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool DeleteSchema<TEntity>() where TEntity : class
        {
            var result = database.DeleteTableAsync(GetName<TEntity>());
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool InsertItem<TEntity>(TEntity entity, string container) where TEntity : class
        {
            var request = CreateAddRequest(container, entity);
            var result = database.PutItemAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool InsertBulk<TEntity>(string container, IList<TEntity> entities) where TEntity : class
        {
            var request = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>> {
                    { container, entities.Select(e => CreateWriteRequest(e)).ToList() }
                }
            };
            
            var result = database.BatchWriteItemAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool DeleteItem<TEntity>(TEntity entity, string container, KeyTypeStore keyStore) where TEntity : class
        {
            var request = CreateDeleteRequest(container, keyStore, entity);
            var result = database.DeleteItemAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool DeleteItem<TEntity>(string container, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var translator = new DynamoQueryProvider(database).GetTranslator(predicate);
            var request = CreateDeleteRequest<TEntity>(container, translator.Members);
            var result = database.DeleteItemAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool UpdateItem<TEntity>(TEntity entity, string container, KeyTypeStore keyStore, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var translator = new DynamoQueryProvider(database).GetTranslator(predicate);
            var request = CreateUpdateRequest(container, keyStore, entity, translator.Members);
            var result = database.UpdateItemAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        internal bool UpdateItem<TEntity>(TEntity entity, string container, KeyTypeStore keyStore, Expression<Func<TEntity, object>>[] properties) where TEntity : class
        {
            var propNames = Reflector.GetPropertyNames(properties);

            var request = CreateUpdateRequest(container, keyStore, entity, propNames);
            var result = database.UpdateItemAsync(request);
            return result.Result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        #endregion

        #region Static helper

        internal static string GetName<TEntity>() => GetRegisteredTypes()[typeof(TEntity)].container;

        #endregion
    }
}
