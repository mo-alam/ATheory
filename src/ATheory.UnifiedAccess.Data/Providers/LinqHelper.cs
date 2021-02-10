/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ATheory.Util.Reflect;
using static ATheory.UnifiedAccess.Data.Providers.ProviderEnums;

namespace ATheory.UnifiedAccess.Data.Providers
{
    public static class LinqHelper
    {
        #region Constructor
        
        static LinqHelper()
        {
            unsupportedLinq = Reflector.GetQueryableFunctions();
            foreach (var name in Enum.GetNames(typeof(LinqMethod)))
            {
                if (unsupportedLinq.Contains(name)) unsupportedLinq.Remove(name);
            }
        }

        #endregion

        #region Private members

        static HashSet<string> unsupportedLinq;
        
        #endregion

        #region Public extension methods

        /// <summary>
        /// Partially evaluates the espression
        /// </summary>
        /// <param name="_">Source expression that would be partially evaluated</param>
        /// <returns>Partially evaluated expression</returns>
        public static Expression EvaluatePartially(this Expression _) => new PartialEvaluator().Evaluate(_);

        /// <summary>
        /// Unquotes the expression, that is unary or the source type expression
        /// </summary>
        /// <param name="_">Instance that would be casted</param>
        /// <returns>Casted instanace</returns>
        public static Expression FirstUnQuotedExpression(this Expression _)
            => _.NodeType == ExpressionType.Quote ? FirstUnQuotedExpression(((UnaryExpression)_).Operand) : _;
        
        /// <summary>
        /// Unquotes the expression, that is unary or of T type expression
        /// </summary>
        /// <typeparam name="T">T type to be casted</typeparam>
        /// <param name="_">Instance that would be casted</param>
        /// <returns>Casted instanace</returns>
        public static T FirstUnQuotedExpression<T>(this Expression _) where T : Expression
            => (T)FirstUnQuotedExpression(_);

        /// <summary>
        /// Finds the method type from the expression
        /// </summary>
        /// <param name="_">MethodCallExpression instance that is to checked</param>
        /// <returns>One of the predefined type</returns>
        public static LinqMethod ToLinqType(this MethodCallExpression _)
            => Enum.TryParse<LinqMethod>(_.Method.Name, out var result) ? result : LinqMethod.None;

        /// <summary>
        /// Checks whether the linq method in the expression is supported or not.
        /// </summary>
        /// <param name="_">MethodCallExpression instance that is to checked </param>
        /// <returns>True if the function being processed is not supported</returns>
        public static bool IsUnsupported(this MethodCallExpression _)
            => unsupportedLinq.Contains(_.Method.Name) && (_.Method.DeclaringType == typeof(Queryable));
        
        /// <summary>
        /// Tests whether the active function is a function with predicate (e.g. 'Where'); default implementation
        /// </summary>
        /// <returns>True if the function can have predicate</returns>
        public static bool IsPredicateFunction(this IQueryTranslator _) => _.ActiveFunction <= LinqMethod.Where;

        public static bool IsSingleValuedLinq(LinqMethod linq)
        {
            return linq switch
            {
                LinqMethod.First => true,
                LinqMethod.FirstOrDefault => true,
                LinqMethod.Last => true,
                LinqMethod.LastOrDefault => true,
                LinqMethod.Single => true,
                LinqMethod.SingleOrDefault => true,
                _ => false
            };
        }

        #endregion
    }
}