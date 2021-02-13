/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.Util.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ATheory.Util.Reflect
{
    public static class Reflector
    {
        #region Constants

        const BindingFlags AllBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        #endregion

        #region Members

        static readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        #endregion

        #region Private methods

        static string GetColName(PropertyInfo propertyInfo)
        {
            var attributeInfo = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            return string.IsNullOrWhiteSpace(attributeInfo?.Name) ? propertyInfo.Name : attributeInfo.Name;
        }

        static string GetName(UnaryExpression unaryExpr)
        {
            return unaryExpr.Operand.NodeType switch
            {
                ExpressionType.MemberAccess => GetName((MemberExpression)unaryExpr.Operand),
                _ => string.Empty,
            };
        }

        static string GetName(MemberExpression memberExpr) => memberExpr.Member.Name;

        static TypeCode GetPureTypeCode(Type type) => Type.GetTypeCode(Nullable.GetUnderlyingType(type)?.UnderlyingSystemType ?? type);

        #endregion

        #region Public methods

        public static TResult InvokeMethod<TClass, TResult>(TClass obj, string name, object[] arguments)
               where TClass : class
        {
            var type = typeof(TClass);
            var method = type.GetMethod(name, AllBinding);
            return (TResult)method?.Invoke(obj, arguments);
        }

        // For static class
        public static TResult InvokeMethod<TResult>(Type type, string name, object[] arguments) 
            => (TResult)type.GetMethod(name, AllBinding)?.Invoke(null, arguments);

        public static Dictionary<string, string> GetProperties<TClass>()
            where TClass : class
        {
            var type = typeof(TClass);
            if (cache.ContainsKey(type.Name))
                return (Dictionary<string, string>)cache[type.Name];

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties == null) return null;
            var result = properties.ToDictionary(p => p.Name, p => GetColName(p));
            cache.Add(type.Name, result);
            return result;
        }

        public static Dictionary<string, PropertyInfo> GetPropertyInfo(Type type, bool useCache = true)
        {
            if (useCache && cache.ContainsKey(type.Name))
                return (Dictionary<string, PropertyInfo>)cache[type.Name];

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (properties == null) return null;
            var result = properties.ToDictionary(p => p.Name, p => p);
            if (useCache)
                cache.Add(type.Name, result);
            return result;
        }

        public static Dictionary<string, PropertyInfo> GetPropertyInfo<TClass>(bool useCache = true)
            => GetPropertyInfo(typeof(TClass), useCache);

        public static Dictionary<string, PropertyInfo> GetPropertyColumnInfo<TClass>(bool makeKeyLower = false)
        {
            var properties = typeof(TClass).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties?.ToDictionary(p => makeKeyLower ? GetColName(p).ToLower() : GetColName(p), p => p);
        }

        public static TEntity SetPropertyValues<TEntity>(Dictionary<string, PropertyInfo> properties, DbDataReader dataReader)
        {
            var entity = Activator.CreateInstance<TEntity>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                var name = dataReader.GetName(i);
                if (!properties.ContainsKey(name)) continue;
                var value = dataReader.GetValue(i);
                if (value != DBNull.Value)
                    properties[name].SetValue(entity, Convert.ChangeType(value, properties[name].PropertyType));
            }
            return entity;
        }

        public static TEntity SetPropertyValue<TEntity>(Dictionary<string, PropertyInfo> properties, object instance)
        {
            var entity = Activator.CreateInstance<TEntity>();
            //for (int i = 0; i < dataReader.FieldCount; i++)
            //{
            //    var name = dataReader.GetName(i);
            //    if (!properties.ContainsKey(name)) continue;
            //    var value = dataReader.GetValue(i);
            //    if (value != DBNull.Value)
            //        properties[name].SetValue(entity, Convert.ChangeType(value, properties[name].PropertyType));
            //}
            return entity;
        }

        /// <summary>
        /// Creates an instance of type TEntity and sets values from a string array
        /// </summary>
        /// <typeparam name="TEntity">Instance type</typeparam>
        /// <param name="properties">Property info</param>
        /// <param name="values">An array of string values</param>
        /// <returns>Instance</returns>
        public static TEntity GetObjectWithValues<TEntity>(IDictionary<int, PropertyInfo> properties, string[] values)
        {
            try
            {
                var entity = Activator.CreateInstance<TEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].IsEmpty()) continue;
                    properties[i].SetValue(entity, Convert.ChangeType(values[i], GetPureTypeCode(properties[i].PropertyType)));
                }
                return entity;
            }
            catch (Exception e) {
                return default;
            }
        }

        public static (string tableName, string schema) GetTableInfo<T>()
        {
            var attributeInfo = typeof(T).GetCustomAttribute<TableAttribute>();
            return (attributeInfo?.Name, attributeInfo?.Schema);
        }

        public static HashSet<string> GetPropertyNames<TSource>(params Expression<Func<TSource, object>>[] properties)
        {
            var result = new HashSet<string>();
            foreach (var property in properties)
            {
                switch (property.Body.NodeType)
                {
                    case ExpressionType.Convert:
                        result.Add(GetName((UnaryExpression)property.Body));
                        break;
                    case ExpressionType.MemberAccess:
                        result.Add(GetName((MemberExpression)property.Body));
                        break;
                }
            }
            return result;
        }

        public static HashSet<string> GetQueryableFunctions() => typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(m => m.Name)
            .ToHashSet();

        public static T GetMemberVariable<T>(object instance, string varName) where T : class => instance
            .GetType()
            .GetField(varName, AllBinding)?
            .GetValue(instance) as T;
        
        #endregion
    }
}
