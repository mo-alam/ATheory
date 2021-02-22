/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.UnifiedAccess.Data.Common;
using System.Collections.Generic;
using static ATheory.UnifiedAccess.Data.Providers.ProviderEnums;

namespace ATheory.UnifiedAccess.Data.Providers
{
    /// <summary>
    /// Interface for query translation
    /// </summary>
    public interface IQueryTranslator
    {
        /// <summary>
        /// Linq function that is currently being evaluated
        /// </summary>
        LinqMethod ActiveFunction { get; set; }

        /// <summary>
        /// Gets/Sets whether the last linq function is single-valued or not.
        /// </summary>
        LinqMethod SingleValuedFunction { get; set; }

        /// <summary>
        /// The resultant object translated by the implementer.
        /// </summary>
        object TranslatedObject { get; }

        /// <summary>
        /// Members used in the query along with variable(if defined) and values.
        /// </summary>
        Dictionary<string, VarValueTuple> Members { get; }

        /// <summary>
        /// Start of a statement (conditional) block
        /// </summary>
        void BlockStart();

        /// <summary>
        /// End of a statement (conditional) block
        /// </summary>
        void BlockEnd();

        /// <summary>
        /// Member is being processed
        /// </summary>
        /// <param name="name">Name of the member</param>
        void TranslateMember(string name);

        /// <summary>
        /// Value is being processed
        /// </summary>
        /// <param name="value">Value of the member</param>
        void TranslateValue(object value);

        /// <summary>
        /// Logical or conditional operator is being processed
        /// </summary>
        /// <param name="type">Type of operator</param>
        void TranslateConditional(ConditionalOperator type);

        /// <summary>
        /// Arithmatic operator is being processed
        /// </summary>
        /// <param name="type">Type of operator</param>
        void TranslateArithmatic(ArithmaticOperator type);

        /// <summary>
        /// Function used on a member is being processed
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="memberName">Name of the member that is calling the function</param>
        /// <param name="argCount">Number of arguments</param>
        void TranslateFunction(string name, string memberName, int argCount);

        /// <summary>
        /// Initialises the object; cleanup, set state, etc.
        /// </summary>
        /// <param name="currentEntity">Registered name of the entity</param>
        void Initialise(string currentEntity);

        /// <summary>
        /// Finalise the expression, that is, mop/set etc.
        /// </summary>
        void Finalise();

        /// <summary>
        /// Gets the translated query string.
        /// </summary>
        /// <returns>Query string</returns>
        string QueryString();
    }
}