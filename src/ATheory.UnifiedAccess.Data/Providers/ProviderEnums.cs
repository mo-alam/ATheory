/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
namespace ATheory.UnifiedAccess.Data.Providers
{
    /// <summary>
    /// Enums related to provider and Linq
    /// </summary>
    public static class ProviderEnums
    {
        /// <summary>
        /// Supported Linq methods...at least for now.
        /// </summary>
        public enum LinqMethod
        {
            None,
            All,                /* bool */
            Any,                /* bool */
            Count,              /* int */
            First,              /* Single value */
            FirstOrDefault,     /* Single value */
            Last,               /* Single value */
            LastOrDefault,      /* Single value */
            LongCount,          /* int */
            Single,             /* Single value */
            SingleOrDefault,    /* Single value */
            Where,              /* Last method to have a predicate */
            Max,                /* Single value */
            Min,                /* Single value */
            OrderBy,
            OrderByDescending,
            Select,
            SelectMany,
            Sum                 /* Numeric */
        }

        public enum OperatorType
        {
            None,           /* None of the supported ones */
            Conditional,    /* Conditional and Logical operator */
            Arithmatic      /* Arithmatic Operator */
        }

        public enum ConditionalOperator
        {
            And,
            AndLogical,
            Or,
            OrLogical,
            Not,
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual
        }

        /// <summary>
        /// Arithmatic operators, a few others were not included...at least for now
        /// </summary>
        public enum ArithmaticOperator
        {
            Add,
            Subtract,
            Multiple,
            Divide,
            Modulo
        }
    }
}