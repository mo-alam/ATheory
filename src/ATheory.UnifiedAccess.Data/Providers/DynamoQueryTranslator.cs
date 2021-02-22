/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using static ATheory.UnifiedAccess.Data.Providers.ProviderEnums;
using static ATheory.UnifiedAccess.Data.Providers.LinqHelper;
using static ATheory.UnifiedAccess.Data.Internal.DynamoPartials;
using ATheory.UnifiedAccess.Data.Common;

namespace ATheory.UnifiedAccess.Data.Providers
{
    /// <summary>
    /// Query translator for DynamoDB
    /// </summary>
    public class DynamoQueryTranslator : IQueryTranslator
    {
        #region Constructor

        public DynamoQueryTranslator() { }

        #endregion

        #region Private members

        QueryRequest request;
        StringBuilder builder;
        string lastVar;
        bool memberVariable;
        Dictionary<string, VarValueTuple> memberVarMap;
        string lastMember;

        #endregion

        #region Private methods

        void AddMemberMap(string memberName, string varName)
        {
            if (!memberVarMap.ContainsKey(memberName))
                memberVarMap.Add(memberName, new VarValueTuple(varName, null));
            lastMember = memberName;
        }

        void SetMemberValue(object value, string memberName = null)
        {
            var name = memberName ?? lastMember;
            if (!memberVarMap.ContainsKey(name)) return;
            memberVarMap[name].Value = value;
        }

        #endregion

        #region Implement interface

        public LinqMethod ActiveFunction { get; set; }

        public LinqMethod SingleValuedFunction { get; set; }

        public object TranslatedObject => request;
        public Dictionary<string, VarValueTuple> Members => memberVarMap;

        public void BlockStart()
        {
            if (this.IsPredicateFunction())
                builder.Append("(");
        }

        public void BlockEnd()
        {
            if (this.IsPredicateFunction())
                builder.Append(")");
        }

        public void TranslateMember(string name)
        {
            if (!this.IsPredicateFunction()) return;

            builder.Append(name);
            lastVar = $":v_{name}";
            memberVariable = true;
            AddMemberMap(name, lastVar);
        }

        public void TranslateValue(object value)
        {
            if (!this.IsPredicateFunction()) return;
            
            if (memberVariable)
                builder.Append(lastVar);

            AttributeValue attributeValue = GetDynamoType(value) switch
            {
                DynamoType.Null => new AttributeValue { NULL = true },
                DynamoType.Number => new AttributeValue { N = value.ToString() },
                DynamoType.Text => new AttributeValue { S = value.ToString() },
                DynamoType.List => new AttributeValue { L = new List<AttributeValue>() },
                _ => throw new NotImplementedException(),
            };

            request.ExpressionAttributeValues.Add(lastVar, attributeValue);
            lastVar = string.Empty;
            SetMemberValue(value);
        }

        public void TranslateConditional(ConditionalOperator type)
        {
            if (this.IsPredicateFunction())
                builder.Append($" {operators[type]} ");
        }

        public void TranslateArithmatic(ArithmaticOperator type) { }

        public void TranslateFunction(string name, string memberName, int argCount)
        {
            if (!this.IsPredicateFunction() || !dynamoFunctions.ContainsKey(name)) return;

            var funcExpr = $"{dynamoFunctions[name]}".Replace("#member#", memberName);
            builder.Append(funcExpr);
            lastVar = $"v_{memberName}";
            memberVariable = false;
        }

        public void Initialise(string currentEntity)
        {
            request = new QueryRequest
            {
                TableName = currentEntity,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
            };
            builder = new StringBuilder();
            lastVar = string.Empty;
            memberVariable = false;
            memberVarMap = new Dictionary<string, VarValueTuple>();
        }

        public void Finalise()
        {
            request.KeyConditionExpression = QueryString();
            if (IsSingleValuedLinq(SingleValuedFunction)) {
                request.Limit = 1;
                if (SingleValuedFunction == LinqMethod.Last || SingleValuedFunction == LinqMethod.LastOrDefault) {
                    request.ScanIndexForward = false;
                }
            }
        }

        public string QueryString() => builder.ToString();

        #endregion

        #region Constants/Enums/Maps

        Dictionary<ConditionalOperator, string> operators = new Dictionary<ConditionalOperator, string> {
            { ConditionalOperator.And, "AND" },
            { ConditionalOperator.AndLogical, "AND" },
            { ConditionalOperator.Or, "OR" },
            { ConditionalOperator.OrLogical, "OR" },
            { ConditionalOperator.Not, "NOT" },
            { ConditionalOperator.Equal, "=" },
            { ConditionalOperator.NotEqual, "<>" },
            { ConditionalOperator.LessThan, "<" },
            { ConditionalOperator.LessThanOrEqual, "<=" },
            { ConditionalOperator.GreaterThan, ">" },
            { ConditionalOperator.GreaterThanOrEqual, ">=" },
        };

        Dictionary<string, string> dynamoFunctions = new Dictionary<string, string> {
            { "Contains", "contains (#member#, :v_#member#)" },
            { "Exists", "attribute_exists (#member#)" },
            { "StartsWith", "begins_with (#member#, :v_#member#)" },
        };

        #endregion
    }
}
