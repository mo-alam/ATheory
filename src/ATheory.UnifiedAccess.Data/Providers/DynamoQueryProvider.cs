/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ATheory.Util.Extensions;
using ATheory.Util.Reflect;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ATheory.UnifiedAccess.Data.Providers
{
    /// <summary>
    /// DynamoDB implementation. V1 => a few more features can still be implemented
    /// </summary>
    public class DynamoQueryProvider : ATrineQueryProvider
    {
        #region Constructor

        public DynamoQueryProvider(IAmazonDynamoDB dynamoDb)
        {
            database = dynamoDb;
            queryTranslator = new DynamoQueryTranslator();
        }

        #endregion

        #region Private members
        
        readonly IAmazonDynamoDB database;

        #endregion

        #region Private methods

        List<object> ReadResults(List<Dictionary<string, AttributeValue>> result)
        {
            var data = new List<object>();
            var propInfo = Reflector.GetPropertyInfo(entityType);

            foreach (var row in result)
            {
                var instance = Activator.CreateInstance(entityType);
                row.ForEach(item => propInfo[item.Key].SetValue(instance, GetValue(propInfo[item.Key], item.Value)));
                data.Add(instance);
            }
            return data;
        }

        object GetValue(PropertyInfo propertyInfo, AttributeValue value)
        {
            return Type.GetTypeCode(propertyInfo.PropertyType) switch
            {
                TypeCode.Int16 => value.N.ToInt16(),
                TypeCode.UInt16 => value.N.ToInt16(),
                TypeCode.Int32 => value.N.ToInt(),
                TypeCode.UInt32 => value.N.ToInt(),
                TypeCode.Int64 => value.N.ToInt64(),
                TypeCode.UInt64 => value.N.ToInt64(),
                TypeCode.Boolean => value.BOOL.ToBool(),
                TypeCode.Char => value.S.Length > 0 ? value.S[0] : Char.MinValue,
                TypeCode.Single => value.N.ToSingle(),
                TypeCode.Double => value.N.ToDouble(),
                TypeCode.Decimal => value.N.ToDecimal(),
                TypeCode.DateTime => value.S.ToDate(DateTime.MinValue),
                TypeCode.String => value.S,
                _ => value.S
            };
        }

        #endregion

        #region Overriden methods

        internal override List<object> ExecTranslatedExpression()
        {
            var request = queryTranslator.TranslatedObject as QueryRequest;
            var response = database.QueryAsync(request);
            var result = response.Result.Items;
            return ReadResults(result);
        }

        #endregion
    }
}
