/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ATheory.UnifiedAccess.Data.Context;
using ATheory.UnifiedAccess.Data.Core;
using ATheory.Util.Extensions;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    /// <summary>
    /// Main entry point for the framework. v1.0
    /// </summary>
    public static class EntityUnifier
    {
        #region Private members

        /// <summary>
        /// Key = Context name; Value = (Life cycle type, resolver function, something)
        /// </summary>
        static IDictionary<string, (LifeCycle life, Func<IUnifiedContext> resolver, object state)> states = new Dictionary<string, (LifeCycle, Func<IUnifiedContext>, object)>(); 
        static readonly Gateway factory = new Gateway();
        static Func<IUnifiedContext> _resolver;

        #endregion

        #region Private methods

        static string GetDefaultName<T>(Connection connection)
        { 
            var name = $"{typeof(T).Name}-default";
            if (states.ContainsKey(name)) {
                name = $"{typeof(T).Name}-{connection.Provider}-default";
                //No more checks, if still exist the consumer should now provide the context name.
            }
            return name;
        }

        #endregion

        #region Internal properties

        internal static ErrorPack Error { get; } = new ErrorPack();

        internal static UnifierOption Option { get; private set; } = new UnifierOption();

        #endregion

        #region Internal methods

        /// <summary>
        /// Gets the context/context resolver dependending on the LifeCycle.
        /// </summary>
        /// <returns>Instance of the context that the calling entity belongs to.</returns>
        internal static IUnifiedContext GetContext() => _resolver();

        /// <summary>
        /// Returns the the list of registered entity info
        /// </summary>
        /// <returns>Registered entity types, Key = entity type, Value = (container, key-property names) </returns>
        internal static IDictionary<Type, (string container, KeyTypeStore keyStore)> GetRegisteredTypes() => factory.RegisteredTypes;

        #endregion

        #region Public methods

        /// <summary>
        /// Entry method to the factory
        /// </summary>
        /// <returns>The factory</returns>
        public static IGateway Factory() => factory;

        /// <summary>
        /// Register the context that'll be used to access the database.
        /// </summary>
        /// <param name="_">Extension</param>
        /// <param name="projection">Projection to DbContext implementing the IUnifiedContext</param>
        /// <param name="contextName">Context name or identifier</param>
        /// <param name="lifeCycle">Life cycle type</param>
        /// <returns>The factory</returns>
        public static IGateway RegisterContext(
            this IGateway _,
            Expression<Func<IUnifiedContext>> projection,
            string contextName,
            LifeCycle lifeCycle = LifeCycle.TransientPerAction)
        {
            if (states.ContainsKey(contextName)) return factory;

            switch (lifeCycle)
            {
                case LifeCycle.TransientPerAction: _resolver = projection.Compile(); break;
                case LifeCycle.SingleInstance:
                    var instance = projection.Compile()();
                    _resolver = () => instance;
                    break;
            }
            factory.ActiveContext = contextName;
            states.Add(contextName, (lifeCycle, _resolver, null));
            return factory;
        }

        /// <summary>
        /// Create the default context that can be used to access the database instead of implementing the DbContext
        /// </summary>
        /// <param name="_">Extension</param>
        /// <param name="connection">The connection object</param>
        /// <param name="contextName">Context name or identifier</param>
        /// <returns>The factory</returns>
        public static IGateway UseDefaultContext(
            this IGateway _,
            Connection connection,
            string contextName = null)
        {
            switch (connection.Provider)
            {
                case StorageProvider.Cosmos:
                    _.RegisterContext(() => new CosmosContext(connection), contextName ?? GetDefaultName<CosmosContext>(connection));
                    break;
                case StorageProvider.Mongo:
                    _.RegisterContext(() => new MongoContext(connection), contextName ?? GetDefaultName<MongoContext>(connection), LifeCycle.SingleInstance);
                    break;
                case StorageProvider.DynamoDb:
                    _.RegisterContext(() => new DynamoContext(connection), contextName ?? GetDefaultName<DynamoContext>(connection), LifeCycle.SingleInstance);
                    break;
                default:
                    _.RegisterContext(() => new ATrineSqlContext(connection), contextName ?? GetDefaultName<ATrineSqlContext>(connection)); /* Defaults are: SqlServer, SqlLite, MySql */
                    break;
            };

            return factory;
        }

        /// <summary>
        /// Use it to make this context the active context when multiple contexts have been registered.
        /// </summary>
        /// <param name="_">Fluent factory</param>
        /// <param name="contextName">Context name or identifier</param>
        /// <returns>The factory</returns>
        public static IGateway SwitchContext(
            this IGateway _,
            string contextName)
        {
            if (contextName.IsExact(factory.ActiveContext)) return factory;
            if (!states.ContainsKey(contextName))
                throw new NotImplementedException();

             _resolver = states[contextName].resolver;
            factory.ActiveContext = contextName;
            return factory;
        }

        /// <summary>
        /// Framework cleanup system
        /// </summary>
        public static void Shutdown()
        {
            foreach (var state in states.Values)
            {
                if (state.life == LifeCycle.SingleInstance && state.resolver() is ISingleLife context)
                {
                    context.Shutdown();
                }
            }
        }

        /// <summary>
        /// Set the callback to receive error.
        /// </summary>
        /// <param name="_">Extension</param>
        /// <param name="callback">Callback</param>
        /// <returns>The factory</returns>
        public static IGateway UseErrorCallback(
            this IGateway _,
            Action<ErrorPack> callback)
        { 
            Error.Set(callback);
            return factory;
        }

        /// <summary>
        /// Get the Error object
        /// </summary>
        /// <param name="_"></param>
        /// <returns>Error object</returns>
        public static ErrorPack GetError(this IGateway _) => Error;

        /// <summary>
        /// Provides option to set various settings regarding memory usage; mostly internal cache.
        /// </summary>
        /// <param name="option">Options to set</param>
        public static void SetOptions(UnifierOption option) => Option = option;

        /// <summary>
        /// Call it to get names of all the registered contexts
        /// </summary>
        /// <returns>List of context names</returns>
        public static List<string> GetContextNames(this IGateway _) => states.Keys.ToList();

        /// <summary>
        /// Get the context name that is at the moment active.
        /// </summary>
        /// <param name="_">The gateway</param>
        /// <returns>Current context name</returns>
        public static string GetActiveContext(this IGateway _) => factory.ActiveContext;

        #endregion
    }
}