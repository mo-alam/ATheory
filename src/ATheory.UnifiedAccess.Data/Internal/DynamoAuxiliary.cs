/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ATheory.Util.Reflect;
using System.Collections.Generic;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;
using static ATheory.UnifiedAccess.Data.Internal.DynamoDbDependencies;

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

        IAmazonDynamoDB database;

        #endregion

        #region Internal methods

        internal bool CreateSchema<TEntity>() where TEntity : class
        {
            var (container, keyStore) = GetRegisteredTypes()[typeof(TEntity)];
            if (!keyStore.HasSpecialKeys) return false;

            var propInfo = Reflector.GetPropertyInfo<TEntity>();
            var attributes = new List<AttributeDefinition>();
            var keys = new List<KeySchemaElement>();
            foreach (var key in keyStore.SpecialKeys)
            {
                var name = key.Value[0];
                attributes.Add(CreateAttributeDefinition(name, propInfo[name]));
                keys.Add(CreateKeySchemaElement(key.Key, name));
            }

            var request = new CreateTableRequest
            {
                TableName = container,
                AttributeDefinitions = attributes,
                KeySchema = keys,
                ProvisionedThroughput = new ProvisionedThroughput(1, 1) /* Default */
            };

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

        #endregion

        #region Static helper

        internal static string GetName<TEntity>() => GetRegisteredTypes()[typeof(TEntity)].container;

        #endregion
    }
}
