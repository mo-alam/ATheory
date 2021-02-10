/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Data;

namespace ATheory.Util.Extensions
{
    public static class Conversions
    {
        #region Public delegate
        
        public delegate bool TryParseProc<T>(string s, out T result);

        #endregion

        #region Public methods

        public static T TryParse<T>(this string _, TryParseProc<T> proc, T failedVal = default(T)) 
            => proc(_, out var r) ? r : failedVal;
        public static T ToType<T>(this DataRow _, string name) => (T)_[name];
        public static T ToType<T>(this DataRow _, int index) => (T)_[index];
        public static Int16 ToInt16(this string _, Int16 defaultValue = 0) => TryParse(_, Int16.TryParse, defaultValue);
        public static Int16 ToInt16(this object _, Int16 defaultValue = 0) => TryParse(_?.ToString(), Int16.TryParse, defaultValue);
        public static int ToInt(this string _, int defaultValue = 0) => TryParse(_, int.TryParse, defaultValue);
        public static int ToInt(this object _, int defaultValue = 0) => TryParse(_?.ToString(), int.TryParse, defaultValue);
        public static Int64 ToInt64(this string _, Int64 defaultValue = 0) => TryParse(_, Int64.TryParse, defaultValue);
        public static Int64 ToInt64(this object _, Int64 defaultValue = 0) => TryParse(_?.ToString(), Int64.TryParse, defaultValue);
        public static bool ToBool(this string _, bool defaultValue = false) => TryParse(_, bool.TryParse, defaultValue);
        public static bool ToBool(this object _, bool defaultValue = false) => TryParse(_?.ToString(), bool.TryParse, defaultValue);
        public static Single ToSingle(this string _, Single defaultValue = 0) => TryParse(_, Single.TryParse, defaultValue);
        public static Single ToSingle(this object _, Single defaultValue = 0) => TryParse(_?.ToString(), Single.TryParse, defaultValue);
        public static double ToDouble(this string _, double defaultValue = 0) => TryParse(_, double.TryParse, defaultValue);
        public static double ToDouble(this object _, double defaultValue = 0) => TryParse(_?.ToString(), double.TryParse, defaultValue);
        public static decimal ToDecimal(this string _, decimal defaultValue = 0) => TryParse(_, decimal.TryParse, defaultValue);
        public static decimal ToDecimal(this object _, decimal defaultValue = 0) => TryParse(_?.ToString(), decimal.TryParse, defaultValue);
        public static DateTime ToDate(this string _, DateTime defaultValue) => TryParse(_, DateTime.TryParse, defaultValue);
        public static DateTime ToDate(this object _, DateTime defaultValue) => TryParse(_?.ToString(), DateTime.TryParse, defaultValue);
        
        #endregion
    }
}
