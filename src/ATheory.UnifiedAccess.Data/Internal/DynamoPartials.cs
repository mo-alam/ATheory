/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ATheory.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Internal
{
    internal static class DynamoPartials
    {
        #region Types/Constants/Enums

        internal enum DynamoType
        {
            Null,
            Number,
            Text,
            List
        }

        #endregion

        #region Internal methods

        internal static DynamoType GetDynamoType(object value) 
            => value == null ? DynamoType.Null : GetDynamoType(value.GetType());

        internal static DynamoType GetDynamoType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return DynamoType.Number;
                case TypeCode.Object:
                    return DynamoType.List;
                default: return DynamoType.Text;
            }
        }

        internal static AttributeDefinition CreateAttributeDefinition(string name, PropertyInfo info)
        {
            var type = GetDynamoType(info.PropertyType);
            return new AttributeDefinition(name,
                type switch
                {
                    DynamoType.Number => ScalarAttributeType.N,
                    DynamoType.List => ScalarAttributeType.B,
                    _ => ScalarAttributeType.S,
                });
        }

        internal static KeySchemaElement CreateKeySchemaElement(SpecialKey key, string name)
        {
            return new KeySchemaElement(name,
                key switch
                {
                    SpecialKey.PartitionKey => KeyType.HASH,
                    _ => KeyType.RANGE,
                });
        }

        internal static AttributeValue CreateAttributeValue(PropertyInfo info, object value)
        {
            return GetDynamoType(info.PropertyType) switch
            {
                DynamoType.Null => new AttributeValue { NULL = true },
                DynamoType.Number => new AttributeValue { N = value.ToString() },
                DynamoType.Text => new AttributeValue { S = value.ToString() },
                DynamoType.List => new AttributeValue { L = new List<AttributeValue>() },
                _ => throw new NotImplementedException()
            };
        }

        //Need to specify this default behaviour
        //Action DELETE and ADD not utilised yet
        internal static AttributeValueUpdate CreateAttributeValueUpdate(PropertyInfo info, object value)
        {
            return new AttributeValueUpdate()
            {
                Action = AttributeAction.PUT,
                Value = CreateAttributeValue(info, value)
            };
        }

        #endregion
    }
}
