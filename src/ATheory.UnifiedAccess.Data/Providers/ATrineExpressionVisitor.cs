using ATheory.Util.Extensions;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Providers.LinqHelper;
using static ATheory.UnifiedAccess.Data.Providers.ProviderEnums;
/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace ATheory.UnifiedAccess.Data.Providers
{
    /// <summary>
    /// Expression visition implemented for query translation. V1.0 
    /// </summary>
    public class ATrineExpressionVisitor : ExpressionVisitor
    {
        #region Constructor

        public ATrineExpressionVisitor() { }
        public ATrineExpressionVisitor(IQueryTranslator queryTranslator) : this() => translator = queryTranslator;

        #endregion

        #region Private members

        IQueryTranslator translator;
        Stack<LinqMethod> linqs = new Stack<LinqMethod>();

        #endregion

        #region Private methods

        void ThrowIfQuerybleButNotDefined(MethodCallExpression node)
        {
            //throw new NotImplementedException();
        }

        OperatorType GetOperatorType(ExpressionType nodeType)
        {
            if (conditional.ContainsKey(nodeType)) return OperatorType.Conditional;
            if (arithmatic.ContainsKey(nodeType)) return OperatorType.Arithmatic;
            return OperatorType.None;
        }

        #endregion

        #region Inter-Translator

        void TranslateMember(MemberExpression node) => translator.TranslateMember(node.Member.Name);
        void TranslateValue(ConstantExpression node) => translator.TranslateValue(node.Value);
        void TranslateOperator(OperatorType operatorType, Expression node)
        {
            switch (operatorType)
            {
                case OperatorType.Conditional: translator.TranslateConditional(conditional[node.NodeType]); break;
                case OperatorType.Arithmatic: translator.TranslateArithmatic(arithmatic[node.NodeType]); break;
            } 
        }

        void TranslateFunctionCall(MethodCallExpression node)
        {
            if (node.Object.NodeType != ExpressionType.MemberAccess) return;
            
            translator.TranslateFunction(node.Method.Name, ((MemberExpression)node.Object).Member.Name, node.Arguments.Count);
            node.Arguments.ForEach(a => Visit(a));
        }

        void TranslateLinqFunctionCall(MethodCallExpression node, LinqMethod linqType)
        {
            if (translator.ActiveFunction != LinqMethod.None)
                linqs.Push(translator.ActiveFunction);
            translator.ActiveFunction = linqType;
            switch (linqType)
            {
                case LinqMethod.FirstOrDefault:
                case LinqMethod.LastOrDefault:
                case LinqMethod.Where:
                case LinqMethod.OrderBy:
                case LinqMethod.OrderByDescending:
                case LinqMethod.Select:
                    Visit(node.Arguments[0]);
                    if (node.Arguments.Count > 1)
                        Visit(node.Arguments[1].FirstUnQuotedExpression<LambdaExpression>().Body);
                    break;
                default:
                    break;
            }
            translator.ActiveFunction = linqs.Count > 0 ? linqs.Pop() : LinqMethod.None;
            if (IsSingleValuedLinq(linqType))
                translator.SingleValuedFunction = linqType;
        }

        #endregion

        #region Overriden methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.IsUnsupported())
                ThrowIfQuerybleButNotDefined(node);
            else
            {
                var linqType = node.ToLinqType();
                switch (linqType) {
                    case LinqMethod.None: TranslateFunctionCall(node); break;
                    default: TranslateLinqFunctionCall(node, linqType); break;
                }
            }
            
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operatorType = GetOperatorType(node.NodeType);
            if (operatorType == OperatorType.None) return base.VisitBinary(node);

            translator.BlockStart();
            Visit(node.Left);
            TranslateOperator(operatorType, node);
            var exp = Visit(node.Right);
            translator.BlockEnd();
            //return exp;
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var operatorType = GetOperatorType(node.NodeType);
            if (operatorType != OperatorType.None)
                TranslateOperator(operatorType, node);
            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            TranslateMember(node);
            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            TranslateValue(node);
            return node;
        }

        #endregion

        #region Expression-Lib mapping

        Dictionary<ExpressionType, ConditionalOperator> conditional = new Dictionary<ExpressionType, ConditionalOperator> {
            { ExpressionType.And, ConditionalOperator.AndLogical },
            { ExpressionType.AndAlso, ConditionalOperator.And },
            { ExpressionType.Or, ConditionalOperator.OrLogical },
            { ExpressionType.OrElse, ConditionalOperator.Or },
            { ExpressionType.Not, ConditionalOperator.Not },
            { ExpressionType.Equal, ConditionalOperator.Equal },
            { ExpressionType.NotEqual, ConditionalOperator.NotEqual },
            { ExpressionType.LessThan, ConditionalOperator.LessThan },
            { ExpressionType.LessThanOrEqual, ConditionalOperator.LessThanOrEqual },
            { ExpressionType.GreaterThan, ConditionalOperator.GreaterThan },
            { ExpressionType.GreaterThanOrEqual, ConditionalOperator.GreaterThanOrEqual }
        };

        Dictionary<ExpressionType, ArithmaticOperator> arithmatic = new Dictionary<ExpressionType, ArithmaticOperator> {
            { ExpressionType.Add, ArithmaticOperator.Add },
            { ExpressionType.Subtract, ArithmaticOperator.Subtract },
            { ExpressionType.Multiply, ArithmaticOperator.Multiple },
            { ExpressionType.Divide, ArithmaticOperator.Divide },
            { ExpressionType.Modulo, ArithmaticOperator.Modulo }
        };

        #endregion
    }
}
