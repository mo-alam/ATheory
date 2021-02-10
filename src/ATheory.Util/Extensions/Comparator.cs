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

        #endregion
    }
}
