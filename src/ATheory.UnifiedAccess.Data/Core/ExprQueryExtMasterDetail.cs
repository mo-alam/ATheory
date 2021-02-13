/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ATheory.UnifiedAccess.Data.Core
{
    public static partial class ExpressionQueryExtension
    {
        #region Master-Detail

        /// <summary>
        /// Filters a sequence of TSource elements based on the predicate if one is provided, otherwise all; in a Master-Detail relationship
        /// </summary>
        /// <typeparam name="TDetailEntity">Detail source type</typeparam>
        /// <param name="child">Expression for the child property. (s => s.Detail)</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <returns>List of TSource elements</returns>
        public static IList<TSource> GetListWithDetail<TSource, TDetailEntity>(
            this IMasterDetailQuery<TSource> _,
            Expression<Func<TSource, ICollection<TDetailEntity>>> detail,
            Expression<Func<TSource, bool>> predicate = null)
            where TSource : class, new() =>
            Get<TSource, IList<TSource>>(c => PredicateIf(c, predicate).Include(detail).ToList());

        #endregion
    }
}
