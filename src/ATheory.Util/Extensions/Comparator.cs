/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using static System.String;

namespace ATheory.Util.Extensions
{
    public static class Comparator
    {
        #region Public methods

        public static bool IsExact(this string _, string rhs) =>
               string.Equals(_, rhs, StringComparison.Ordinal);
        public static bool IsEmptyOrExact(this string _, string rhs) =>
            IsNullOrWhiteSpace(_) || string.Equals(_, rhs, StringComparison.Ordinal);
        public static bool Alike(this string _, string rhs) =>
            string.Equals(_, rhs, StringComparison.OrdinalIgnoreCase);
        public static bool EmptyOrAlike(this string _, string rhs) =>
            IsNullOrWhiteSpace(_) || string.Equals(_, rhs, StringComparison.OrdinalIgnoreCase);
        public static bool IsEmpty(this string _) => IsNullOrWhiteSpace(_);
        public static string OtherIfThisEmpty(this string _, string other) 
            => IsNullOrWhiteSpace(_) ? other : _;
        public static bool IsDefault(this object _)
        {
            if (_ == null) return true;
            return Type.GetTypeCode(_.GetType()) switch
            {
                TypeCode.Boolean => Equals(_, default(bool)),
                TypeCode.Char => Equals(_, default(char)),
                TypeCode.SByte => Equals(_, default(sbyte)),
                TypeCode.Byte => Equals(_, default(byte)),
                TypeCode.Int16 => Equals(_, default(Int16)),
                TypeCode.UInt16 => Equals(_, default(UInt16)),
                TypeCode.Int32 => Equals(_, default(int)),
                TypeCode.UInt32 => Equals(_, default(uint)),
                TypeCode.Int64 => Equals(_, default(Int64)),
                TypeCode.UInt64 => Equals(_, default(UInt64)),
                TypeCode.Single => Equals(_, default(Single)),
                TypeCode.Double => Equals(_, default(double)),
                TypeCode.Decimal => Equals(_, default(decimal)),
                TypeCode.DateTime => Equals(_, default(DateTime)),
                TypeCode.String => Equals(_, default(string)),
                _ => false
            };
        }

        #endregion
    }
}
