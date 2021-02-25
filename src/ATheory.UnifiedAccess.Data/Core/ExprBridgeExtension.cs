/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using ATheory.UnifiedAccess.Data.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Core.ServiceEnums;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class ExprBridgeExtension
    {
        #region Private methods

        static void Switch(bool isLeft) 
            => Factory().SwitchContext(isLeft ? AccessorServices.BridgeInfo.LeftContext : AccessorServices.BridgeInfo.RightContext);

        static BridgeResult ExecuteFunction<TLeft, TRight, TReader>(bool pushAction, 
            Expression<Func<TLeft, TRight>> projection,
            Func<TReader> funcReader,
            Func<TReader, Func<TLeft, TRight>, bool> funcWriter)
        {
            if(!pushAction) Switch(false);

            var reader = funcReader();
            if (reader == null)
            {
                return Error.HasError ? BridgeResult.ErrorRead : BridgeResult.EmptyRead;
            }
            var convert = projection.Compile();

            Switch(!pushAction);

            var success = funcWriter(reader, convert);

            if (pushAction) Switch(true);

            return success ? BridgeResult.Success : BridgeResult.ErrorWrite;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Push the first result to the right context
        /// </summary>
        /// <typeparam name="TLeft">Source entity type</typeparam>
        /// <typeparam name="TRight">Destination entity type</typeparam>
        /// <param name="_">Bridge</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="projection">A function to copy convert TLeft element to TRight element</param>
        /// <returns>One of BridgeResult</returns>
        public static BridgeResult Push<TLeft, TRight>(this IBridge _,
            Expression<Func<TLeft, bool>> predicate,
            Expression<Func<TLeft, TRight>> projection)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return ExecuteFunction(true, projection,
                () => ExpressionQueryExtension.GetFirst(null, predicate), 
                (r, f) => ExpressionQueryExtension.Insert(null, f(r)));
        }

        /// <summary>
        /// Push the list result to the right context
        /// </summary>
        /// <typeparam name="TLeft">Source entity type</typeparam>
        /// <typeparam name="TRight">Destination entity type</typeparam>
        /// <param name="_">Bridge</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="projection">A function to copy convert TLeft element to TRight element</param>
        /// <returns>One of BridgeResult</returns>
        public static BridgeResult PushMany<TLeft, TRight>(this IBridge _,
            Expression<Func<TLeft, bool>> predicate,
            Expression<Func<TLeft, TRight>> projection)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return ExecuteFunction(true, projection,
                   () => ExpressionQueryExtension.GetList(null, predicate),
                   (r, f) => ExpressionQueryExtension.InsertBulk(null, r.Select(l => f(l)).ToList()));
        }

        /// <summary>
        /// Push the first result from the right context and pushes it to the left context
        /// </summary>
        /// <typeparam name="TLeft">Destination entity type</typeparam>
        /// <typeparam name="TRight">Source entity type</typeparam>
        /// <param name="_">Bridge</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="projection">A function to copy convert TRight element to TLeft element</param>
        /// <returns>One of BridgeResult</returns>
        public static BridgeResult Pull<TLeft, TRight>(this IBridge _,
            Expression<Func<TRight, bool>> predicate,
            Expression<Func<TRight, TLeft>> projection)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return ExecuteFunction(false, projection,
                   () => ExpressionQueryExtension.GetFirst(null, predicate),
                   (r, f) => ExpressionQueryExtension.Insert(null, f(r)));
        }

        /// <summary>
        /// Push the all the results from the right context and pushes them to the left context
        /// </summary>
        /// <typeparam name="TLeft">Destination entity type</typeparam>
        /// <typeparam name="TRight">Source entity type</typeparam>
        /// <param name="_">Bridge</param>
        /// <param name="predicate">A function for a condition to filter elements. (s => s.Id) </param>
        /// <param name="projection">A function to copy convert TRight element to TLeft element</param>
        /// <returns>One of BridgeResult</returns>
        public static BridgeResult PullMany<TLeft, TRight>(this IBridge _,
            Expression<Func<TRight, bool>> predicate,
            Expression<Func<TRight, TLeft>> projection)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return ExecuteFunction(false, projection,
                   () => ExpressionQueryExtension.GetList(null, predicate),
                   (r, f) => ExpressionQueryExtension.Insert(null, r.Select(l => f(l)).ToList()));
        }

        #endregion
    }
}