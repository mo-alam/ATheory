/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Core
{
    /// <summary>
    /// An extension for expression queries. Any class can inherit IQueryExecutor<TSource>
    /// and get all the functions exposed by this extension class.
    /// </summary>
    public static partial class ExpressionQueryExtension
    {
        #region Private methods        

        static TResult Exec<TResult>(
            Func<IUnifiedContext, TResult> func)
        {
            Error.Clear();
            try
            {
                using var _context = GetContext();
                return func(_context);
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return default;
            }
        }

        static TResult Get<TSource, TResult>(
            Func<IQueryable<TSource>, TResult> func)
            where TSource : class, new()
        {
            Error.Clear();
            try
            {
                using var _context = GetContext();
                return func(_context.EntitySet<TSource>());
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return default;
            }
        }

        static IQueryable<TSource> PredicateIf<TSource>(
            IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
            where TSource : class, new() =>
            predicate == null ? source : source.Where(predicate);

        #endregion

        #region Public extension method (Read)

        /// <summary>
        /// Use it to pass any expression that is exposed by IQueryable
        /// </summary>
        /// <typeparam name="TSource">Type of the source object</typeparam>
        /// <typeparam name="TResult">Type of the returned result</typeparam>
        /// <param name="_">Caller</param>
        /// <param name="func">Function to be called from the IQueryable</param>
        /// <returns>Result of type TResult</returns>
        public static TResult ExecQueryable<TSource, TResult>(
            this IReadQuery<TSource> _,
            Func<IQueryable<TSource>, TResult> func)
            where TSource : class, new() =>
            Get(func);

        /// <summary>
        /// Fetches the first record in the sequence
        /// </summary>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>Entity</returns>
        public static TSource GetFirst<TSource>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate)
            where TSource : class, new() =>
            Get<TSource, TSource>(a => a.FirstOrDefault(predicate));

        /// <summary>
        /// Fetches the first record in the sequence and returns a DTO of TSelect type
        /// </summary>
        /// <typeparam name="TSelect">Type of the DTO</typeparam>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <returns>TSelect instance</returns>
        public static TSelect GetFirst<TSource, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TSelect>> selector)
            where TSource : class, new() =>
            selector.Compile()(_.GetFirst(predicate));

        /// <summary>
        /// Fetches the last record in the sequence
        /// </summary>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>Entity</returns>
        public static TSource GetLast<TSource>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate)
            where TSource : class, new() =>
            Get<TSource, TSource>(a => a.LastOrDefault(predicate));

        /// <summary>
        /// Fetches the last record in the sequence and returns a DTO of TSelect type
        /// </summary>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <returns>TSelect instance</returns>
        public static TSelect GetLast<TSource, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TSelect>> selector)
            where TSource : class, new() =>
            selector.Compile()(_.GetLast(predicate));

        /// <summary>
        /// Fetches the first record in the sequence ordered by the key in descending order 
        /// </summary>
        /// <typeparam name="TKey">Type of the selector key</typeparam>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <returns>Entity</returns>
        public static TSource GetTop<TSource, TKey>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TKey>> keySelector)
            where TSource : class, new() =>
            Get<TSource, TSource>(f => f.Where(predicate).OrderByDescending(keySelector).FirstOrDefault());

        /// <summary>
        /// Fetches the first record in the sequence ordered by the key in descending order 
        /// </summary>
        /// <typeparam name="TKey">Type of the selector key</typeparam>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <returns>Entity</returns>
        public static TSelect GetTop<TSource, TKey, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TSelect>> selector)
            where TSource : class, new() =>
            selector.Compile()(_.GetTop(predicate, keySelector));

        /// <summary>
        /// Fetches the first record in the sequence ordered by the key in ascending order 
        /// </summary>
        /// <typeparam name="TKey">Type of the selector key</typeparam>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <returns>Entity</returns>
        public static TSource GetBottom<TSource, TKey>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TKey>> keySelector)
            where TSource : class, new() =>
            Get<TSource, TSource>(f => f.Where(predicate).OrderBy(keySelector).FirstOrDefault());

        /// <summary>
        /// Fetches the first record in the sequence ordered by the key in ascending order 
        /// </summary>
        /// <typeparam name="TKey">Type of the selector key</typeparam>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <returns>Entity</returns>
        public static TSelect GetBottom<TSource, TKey, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TSelect>> selector)
            where TSource : class, new() =>
            selector.Compile()(_.GetBottom(predicate, keySelector));

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all.
        /// </summary>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetList<TSource>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSource>>(c => PredicateIf(c, predicate).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all.
        /// </summary>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSelect> GetList<TSource, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, TSelect>> selector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSelect>>(c => PredicateIf(c, predicate).Select(selector).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all. Orders by asc.
        /// </summary>
        /// <typeparam name="TKey">Type of the key selector</typeparam>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetOrderedList<TSource, TKey>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSource>>(c => PredicateIf(c, predicate).OrderBy(keySelector).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all. Orders by asc.
        /// </summary>
        /// <typeparam name="TKey">Type of the key selector</typeparam>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSelect> GetOrderedList<TSource, TKey, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TSelect>> selector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSelect>>(c => PredicateIf(c, predicate).OrderBy(keySelector).Select(selector).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all. Orders by desc.
        /// </summary>
        /// <typeparam name="TKey">Type of the key selector</typeparam>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetDescendingOrderedList<TSource, TKey>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSource>>(c => PredicateIf(c, predicate).OrderByDescending(keySelector).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all. Orders by desc.
        /// </summary>
        /// <typeparam name="TKey">Type of the key selector</typeparam>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSelect> GetDescendingOrderedList<TSource, TKey, TSelect>(
            this IReadQuery<TSource> _,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TSelect>> selector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSelect>>(c => PredicateIf(c, predicate).OrderByDescending(keySelector).Select(selector).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided otherwise all and returns only the elements within the range. 
        /// </summary>
        /// <param name="range">Range: from = 0 based element in the sequence; count = total number of elements</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetRange<TSource>(
            this IReadQuery<TSource> _,
            (int from, int count) range,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSource>>(c => PredicateIf(c, predicate).Skip(range.from).Take(range.count).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided otherwise all and returns only the elements within the range. 
        /// </summary>
        /// <param name="range">Range: from = 0 based element in the sequence; count = total number of elements</param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSelect> GetRange<TSource, TSelect>(
            this IReadQuery<TSource> _,
            (int from, int count) range,
            Expression<Func<TSource, TSelect>> selector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSelect>>(c => PredicateIf(c, predicate).Skip(range.from).Take(range.count).Select(selector).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided otherwise all and returns only the elements within the range. 
        /// </summary>
        /// <param name="range">Range: from = 0 based element in the sequence; count = total number of elements</param>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetRangeOrderBy<TSource, TKey>(
            this IReadQuery<TSource> _,
            (int from, int count) range,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSource>>(c => PredicateIf(c, predicate).OrderBy(keySelector).Skip(range.from).Take(range.count).ToList());

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided otherwise all and returns only the elements within the range. 
        /// </summary>
        /// <param name="range">Range: from = 0 based element in the sequence; count = total number of elements</param>
        /// <param name="keySelector">A function for a property to order by</param>
        /// <param name="selector">A function to copy TSource elements to TSelect element</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSelect> GetRangeOrderBy<TSource, TKey, TSelect>(
            this IReadQuery<TSource> _,
            (int from, int count) range,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TSelect>> selector,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSelect>>(c => PredicateIf(c, predicate).OrderBy(keySelector).Skip(range.from).Take(range.count).Select(selector).ToList());

        #endregion

        #region Public extension method (Write: Insert, Update, Delete)

        /// <summary>
        /// Inserts a new entity in the database
        /// </summary>
        /// <param name="entity">Entity to be pushed in to the database</param>
        /// <returns>Success or failure</returns>
        public static bool Insert<TSource>(
            this IWriteQuery<TSource> _,
            TSource entity)
            where TSource : class, new() =>
            Exec(c => c.Insert(entity));

        /// <summary>
        /// Updates the entity, affects only the columns if specified in properties otherwise all. Not to be used for MongoDB
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        /// <param name="properties">Array of properties that would be updated, if none is provided the whole entity will be updated</param>
        /// <returns>Success or failure</returns>
        public static bool Update<TSource>(
            this IWriteQuery<TSource> _,
            TSource entity,
            params Expression<Func<TSource, object>>[] properties)
            where TSource : class, new() =>
            Exec(c => c.Update(entity, properties));

        /// <summary>
        /// Updates the entity. Not to be used for MongoDB
        /// </summary>
        /// <param name="predicate">A function for a condition to update elements.</param>
        /// <param name="entity">Entity to be updated</param>
        /// <returns>Success or failure</returns>
        public static bool Update<TSource>(
            this IWriteQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate,
            TSource entity)
            where TSource : class, new() =>
            Exec(c => c.Update(predicate, entity));

        /// <summary>
        /// Deletes the entity from the database
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>Success or failure</returns>
        public static bool Delete<TSource>(
            this IWriteQuery<TSource> _,
            TSource entity)
            where TSource : class, new() =>
            Exec(c => c.Delete(entity));

        /// <summary>
        /// Delete the entity(s) from the database
        /// </summary>
        /// <param name="predicate">A function for a condition to delete elements.</param>
        /// <returns>Success or failure</returns>
        public static bool Delete<TSource>(
            this IWriteQuery<TSource> _,
            Expression<Func<TSource, bool>> predicate)
            where TSource : class, new() =>
            Exec(c => c.Delete(predicate));

        /// <summary>
        /// Inserts in bulk, the entire data table
        /// </summary>
        /// <param name="list">List of TSource elements that'll be inserted in to the table</param>
        /// <returns>Success or failure</returns>
        public static bool InsertBulk<TSource>(
            this IWriteQuery<TSource> _,
            IList<TSource> sources)
            where TSource : class, new() =>
            Exec(c => c.InsertBulk(sources));

        #endregion
    }
}