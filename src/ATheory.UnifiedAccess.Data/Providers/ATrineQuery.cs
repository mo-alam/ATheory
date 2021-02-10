/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ATheory.UnifiedAccess.Data.Providers
{
    public class ATrineQuery<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        #region Constructor

        public ATrineQuery(ATrineQueryProvider queryProvider)
        {
            provider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            expression = Expression.Default(typeof(ATrineQuery<T>));
        }

        public ATrineQuery(ATrineQueryProvider queryProvider, Expression queyExpression)
        {
            if (queyExpression == null) throw new ArgumentNullException(nameof(queyExpression));
            if (!typeof(IQueryable<T>).IsAssignableFrom(queyExpression.Type))
                throw new ArgumentOutOfRangeException(nameof(queyExpression));
            provider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            expression = queyExpression;
        }

        #endregion

        #region Private members

        readonly ATrineQueryProvider provider;
        readonly Expression expression;

        #endregion

        #region Public properties
        
        public Expression Expression => expression; 

        #endregion

        #region Implement IQueryable interface

        Type IQueryable.ElementType => typeof(T);
        Expression IQueryable.Expression => expression;
        IQueryProvider IQueryable.Provider => provider;

        #endregion

        #region Implement interface (IEnumerable<T>, IEnumerable)

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)provider.ExecuteExpression(expression)).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)provider.ExecuteExpression(expression)).GetEnumerator();

        #endregion

        #region Overriden methods

        public override string ToString() => provider.GetString(expression); 

        #endregion
    }
}
