/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace ATheory.UnifiedAccess.Data.Common
{
    public class VarValueTuple<TVar, TValue>
    {
        #region Constructor

        public VarValueTuple(TVar variable, TValue value)
        {
            Variable = variable;
            Value = value;
        }

        #endregion

        #region Properties

        public TVar Variable { get; set; }
        public TValue Value { get; set; }
        public bool HasValue => !Equals(Value, default(TValue));

        #endregion
    }

    public class VarValueTuple<TValue> : VarValueTuple<string, TValue>
    {
        #region Constructor

        public VarValueTuple(string variable, TValue value) : base(variable, value)
        {
        }

        #endregion
    }

    public class VarValueTuple : VarValueTuple<string, object>
    {
        #region Constructor

        public VarValueTuple(string variable, object value) : base(variable, value)
        {
        }

        #endregion
    }
}