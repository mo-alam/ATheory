/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using ATheory.UnifiedAccess.Data.Core;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    public static class AccessorServices
    {
        #region Internal members

        internal static BridgingVars BridgeInfo = new BridgingVars();

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the query service interface. Supported interfaces are:
        /// 1. IReadQuery<TSource>
        /// 2. IWriteQuery<TSource>
        /// 3. IMasterDetailQuery<TSource>
        /// 4. ISchemaQuery<TSource>
        /// 5. ISqlQuery
        /// </summary>
        /// <typeparam name="TQuery">One of the supported interfaces</typeparam>
        /// <param name="_">Factory instance</param>
        /// <returns>Query service interface</returns>
        public static TQuery GetQueryService<TQuery>(this IGateway _)
            where TQuery : IQueryService
            => (TQuery)(IQueryService)null;

        /// <summary>
        /// Gets the reader query interface for the entity
        /// </summary>
        /// <typeparam name="TSource">Type of the entity</typeparam>
        /// <param name="_">The gateway</param>
        /// <returns>Reader interface</returns>
        public static IReadQuery<TSource> GetReader<TSource>(this IGateway _)
            where TSource : class, new()
            => GetQueryService<IReadQuery<TSource>>(_);

        /// <summary>
        /// Gets the writer query interface for the entity
        /// </summary>
        /// <typeparam name="TSource">Type of the entity</typeparam>
        /// <param name="_">The gateway</param>
        /// <returns>Writer interface</returns>
        public static IWriteQuery<TSource> GetWriter<TSource>(this IGateway _)
            where TSource : class, new()
            => GetQueryService<IWriteQuery<TSource>>(_);

        /// <summary>
        /// Gets the schema query interface for the entity
        /// </summary>
        /// <typeparam name="TSource">Type of the entity</typeparam>
        /// <param name="_">The gateway</param>
        /// <returns>Schema interface</returns>
        public static ISchemaQuery<TSource> GetSchema<TSource>(this IGateway _)
            where TSource : class, new()
            => GetQueryService<ISchemaQuery<TSource>>(_);

        /// <summary>
        /// Gets the MasterDetail query interface for the entity
        /// </summary>
        /// <typeparam name="TSource">Type of the entity</typeparam>
        /// <param name="_">The gateway</param>
        /// <returns>MasterDetail interface</returns>
        public static IMasterDetailQuery<TSource> GetMasterDetail<TSource>(this IGateway _)
            where TSource : class, new()
            => GetQueryService<IMasterDetailQuery<TSource>>(_);

        /// <summary>
        /// Gets the Sql query interface for the entity
        /// </summary>
        /// <typeparam name="TSource">Type of the entity</typeparam>
        /// <param name="_">The gateway</param>
        /// <returns>Sql interface</returns>
        public static ISqlQuery GetSql(this IGateway _)
            => GetQueryService<ISqlQuery>(_);

        /// <summary>
        /// Creates a bridge (tunnel) beween the current context (as left) and the right context
        /// </summary>
        /// <param name="_">The gateway</param>
        /// <param name="rightContext">Conext name to establish a data tunnel</param>
        /// <returns>The interface instance</returns>
        public static IBridge Bridge(this IGateway _, string rightContext)
            => Bridge(_, _.GetActiveContext(), rightContext);

        /// <summary>
        /// Creates a bridge (tunnel) beween the left context and the right context
        /// </summary>
        /// <param name="_">The gateway</param>
        /// <param name="leftContext"></param>
        /// <param name="rightContext"></param>
        /// <returns></returns>
        public static IBridge Bridge(this IGateway _, string leftContext, string rightContext)
        {
            BridgeInfo.Set(_.GetActiveContext(), leftContext, rightContext);
            _.SwitchContext(leftContext);
            return null;
        }

        /// <summary>
        /// Remove bridging 
        /// </summary>
        /// <param name="_">The gateway</param>
        public static void UnBridge(this IGateway _) {
            _.SwitchContext(BridgeInfo.CurrentContext);
            BridgeInfo.Remove();
        }

        #endregion

        #region Internal structures

        internal class BridgingVars
        {
            internal string CurrentContext;
            internal string LeftContext;
            internal string RightContext;
            internal bool HasBridge;

            internal void Set(string current, string leftContext, string rightContext)
            {
                CurrentContext = current;
                LeftContext = leftContext;
                RightContext = rightContext;
                HasBridge = true;
            }

            internal void Remove()
            {
                CurrentContext = LeftContext = RightContext = string.Empty;
                HasBridge = false;
            }
        }

        #endregion
    }
}