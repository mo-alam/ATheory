/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.UnifiedAccess.Data.Core;
using System.Linq;

namespace ATheory.UnifiedAccess.Data.Internal
{
    internal static class CommonExtension
    {
        /// <summary>
        /// Cast to specified type derived from IUnifiedContext
        /// </summary>
        /// <typeparam name="TContext">Target type derived from IUnifiedContext</typeparam>
        /// <param name="_">Instance to be cast</param>
        /// <returns>Object casted to the type</returns>
        internal static TContext ToType<TContext>(this IUnifiedContext _) where TContext : IUnifiedContext => (TContext)_;

        /// <summary>
        /// Cast to specified type derived from IQueryable<T>
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <typeparam name="TQueryable">Target type derived from IQueryable<T></typeparam>
        /// <param name="_">Instance to be cast</param>
        /// <returns>Object casted to the type</returns>
        internal static TQueryable ToType<T,TQueryable>(this IQueryable<T> _) where TQueryable : IQueryable<T> => (TQueryable)_;
    }
}
