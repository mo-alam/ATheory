/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Providers.LinqHelper;

namespace ATheory.UnifiedAccess.Data.Providers
{

    /// <summary>
    /// Base IQueryProvider implementation for the library
    /// </summary>
    public abstract class ATrineQueryProvider : IQueryProvider
    {
        #region Private members

        string currentEntity;

        #endregion

        #region Protected members
        
        protected IQueryTranslator queryTranslator;
        protected Type entityType;

        #endregion

        #region Private methods

        void CreatVisitorAndVisit(Expression expression)
        {
            queryTranslator.Initialise(currentEntity);
            var expressionVisitor = new ATrineExpressionVisitor(queryTranslator);
            expressionVisitor.Visit(expression);
            queryTranslator.Finalise();
        }

        #endregion

        #region Implement interface (Explicit)

        /// <summary>
        /// Creates an IQueryable<T> instance of T Element.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="expression">Expression that would be used in the query</param>
        /// <returns>IQueryable<T> instance</returns>
        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            entityType = typeof(T);
            currentEntity = entityType.Name;
            return new ATrineQuery<T>(this, expression);
        }

        /// <summary>
        /// Creates an IQueryable instance.
        /// </summary>
        /// <param name="expression">Expression that would be used in the query</param>
        /// <returns>IQueryable instance</returns>
        IQueryable IQueryProvider.CreateQuery(Expression expression) =>
            (IQueryable)Activator.CreateInstance(
                typeof(ATrineQuery<>).MakeGenericType(expression.GetSequenceType()),
                new object[] { this, expression });

        /// <summary>
        /// Executes the expression.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="expression">Expression that would be used in the query</param>
        /// <returns>Instance of T element or defult(T) based on the filter</returns>
        T IQueryProvider.Execute<T>(Expression expression) => (T)ExecuteExpression(expression);

        /// <summary>
        /// Executes the expression.
        /// </summary>
        /// <param name="expression">Expression that would be used in the query</param>
        /// <returns>Instance or null based on the filter</returns>
        object IQueryProvider.Execute(Expression expression) => ExecuteExpression(expression);

        #endregion

        #region Internal methods

        internal IQueryable<T> CreateQuery<T>(string entityName)
        {
            entityType = typeof(T);
            currentEntity = entityName;
            return new ATrineQuery<T>(this);
        }

        /// <summary>
        /// Returns the expression as converted string
        /// </summary>
        /// <param name="expression">Expression passed in</param>
        /// <returns>String</returns>
        internal virtual string GetExpressionString(Expression expression)
        {
            CreatVisitorAndVisit(expression);
            return queryTranslator.QueryString();
        }
        
        /// <summary>
        /// Executes the expression to fetch result
        /// </summary>
        /// <param name="expression">Expression passed in</param>
        /// <returns>Resultant object/value</returns>
        internal virtual object ExecExpression(Expression expression)
        {
            CreatVisitorAndVisit(expression);
            var result = ExecTranslatedExpression();
            if (result == null || result.Count == 0) return null;
            return IsSingleValuedLinq(queryTranslator.SingleValuedFunction)
                ? result[0]
                : result;
        }

        /// <summary>
        /// Derived class must implement this method to actually execute the translated query and return result;
        /// </summary>
        /// <returns>Queried data</returns>
        internal abstract List<object> ExecTranslatedExpression();

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the expression as converted string
        /// </summary>
        /// <param name="expression">Expression passed in</param>
        /// <returns>String</returns>
        public string GetString(Expression expression) => GetExpressionString(expression.EvaluatePartially());

        /// <summary>
        /// Executes the expression to fetch result
        /// </summary>
        /// <param name="expression">Expression passed in</param>
        /// <returns>Resultant object/value</returns>
        public object ExecuteExpression(Expression expression) => ExecExpression(expression.EvaluatePartially());

        #endregion
    }
}
