/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Amazon.DynamoDBv2.DocumentModel;
using ATheory.UnifiedAccess.Data.Helper;
using System.Reflection;

namespace ATheory.UnifiedAccess.Data.Providers
{
    /*
     * We could create a simple implementation or we could implement the whole IQueryable<T>, IQueryProvider<T> linq infrastructure
     */
    public static class ProviderExtension
    {
        // If reflection proves to be inefficient calling all the time I'll implement a ceche later
        internal static Document CreateSetDocument<T>(this Table _, T entity)
        {
            var type = typeof(T);
            
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties == null) return null;
            var document = new Document();
            foreach (var property in properties) {
                document[property.Name] = property.GetValue(entity).ToEntry();
            }
            return document;
        }
    }
}