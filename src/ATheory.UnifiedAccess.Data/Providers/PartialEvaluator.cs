/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ATheory.UnifiedAccess.Data.Providers
{
    // Partial evaluation idea has been borrowed from MSFT
    public class PartialEvaluator : ExpressionVisitor
    {
        #region Private members

        Func<Expression, Expression> visitorFunc;
        HashSet<Expression> partialList = new HashSet<Expression>();
        Stack<Expression> skipEvaluation = new Stack<Expression>();

        #endregion
                
        #region Overriden methods

        public override Expression Visit(Expression expression) => visitorFunc(expression);

        #endregion

        #region Private methods

        Expression Convert(Expression expression) =>
            (expression.NodeType == ExpressionType.Constant)
            ? expression
            : Expression.Constant(Expression.Lambda(expression).Compile().DynamicInvoke(null), expression.Type);
        

        Expression EvaluateVisitor(Expression expression) => 
            (expression == null)
            ? null 
            : partialList.Contains(expression) ? Convert(expression) : base.Visit(expression);
        
        Choice EvaluateChoice(ExpressionType nodeType) {
            switch (nodeType) {
                case ExpressionType.Parameter: return Choice.Source;
                case ExpressionType.MemberAccess:
                case ExpressionType.Constant:
                    return Choice.Add;
                default: return Choice.Ignore;
            }
        }

        Expression ListProducerVisitor(Expression expression)
        {
            if (expression == null) return null;

            base.Visit(expression);

            switch (EvaluateChoice(expression.NodeType)) {
                case Choice.Add:
                    {
                        if (skipEvaluation.Count > 0) skipEvaluation.Pop();
                        else
                            partialList.Add(expression);
                    }
                    break;
                case Choice.Source: skipEvaluation.Push(expression); break;
            }
            
            return expression;
        }

        #endregion

        #region Internal methods

        internal Expression Evaluate(Expression expression)
        {
            visitorFunc = ListProducerVisitor;
            Visit(expression);
            visitorFunc = EvaluateVisitor;
            return Visit(expression);
        }

        #endregion

        #region Private enums

        enum Choice
        {
            Ignore,
            Add,
            Source
        }

        #endregion
    }
}