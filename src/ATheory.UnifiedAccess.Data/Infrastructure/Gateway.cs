/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.Util.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    class Gateway : IGateway
    {
        #region Private members

        /// <summary>
        /// Key     = Context-Name
        /// Value   = Registered Types [Dictionary]
        /// </summary>
        IDictionary<string, IDictionary<Type, (string, KeyTypeStore)>> store = new Dictionary<string, IDictionary<Type, (string, KeyTypeStore)>>();
        string activeContext;
        Type currentType;

        #endregion

        #region Implement interface

        public IGateway Register<TEntity>(
            params Expression<Func<TEntity, object>>[] keys) 
            => Register(NoContainer, keys);

        public IGateway Register<TEntity>(
            string container, 
            params Expression<Func<TEntity, object>>[] keys)
        {
            RegisteredTypes.Add(typeof(TEntity), (container, new KeyTypeStore { Keys = GetProperties(keys) }));
            currentType = typeof(TEntity);
            return this;
        }

        //Theoretically it is possible to have same collection name and type in different container. But for now, not considering
        public IGateway Register<TEntity>(
            string collectionName,
            string container = null)
        {
            RegisteredTypes.Add(typeof(TEntity), (collectionName, new KeyTypeStore { Container = container }));
            currentType = typeof(TEntity);
            return this;
        }

        public IGateway SpecialKey(string key, SpecialKey keyType = TypeCatalogue.SpecialKey.PartitionKey)
        {
            if (currentType == null) return this;
            RegisteredTypes[currentType].KeyStore.AddSpecialKey(keyType, key);
            return this;
        }

        #endregion

        #region Internal properties

        /// <summary>
        /// Key     = Entity type
        /// Value   = Tuple([Container | Collection | null], [Key-properties])
        /// </summary>
        internal IDictionary<Type, (string Container, KeyTypeStore KeyStore)> RegisteredTypes { get; private set; }

        /// <summary>
        /// Active context
        /// </summary>
        internal string ActiveContext
        {
            get => activeContext; 
            set {
                activeContext = value;
                if (!store.ContainsKey(activeContext)) store.Add(value, new Dictionary<Type, (string, KeyTypeStore)>());
                RegisteredTypes = store[activeContext];
            }
        }

        #endregion

        #region Private methods

        string[] GetProperties<TEntity>(Expression<Func<TEntity, object>>[] keys)
            => (keys == null || keys.Length < 1) ? null : Reflector.GetPropertyNames(keys).ToArray();

        #endregion
    }
}