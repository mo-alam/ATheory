/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;


namespace ATheory.Util.Extensions
{
    public static class Collections
    {
        public static void ForEach<T>(this T[] _, Action<T> callback)
        {
            foreach (T t in _) callback(t);
        }

        public static void ForEach<TKey, TValue>(
            this IDictionary<TKey, TValue> _,
            Action<KeyValuePair<TKey, TValue>> callback)
        {
            foreach (var pair in _) callback(pair);
        }

        public static void ForEach<T>(this IEnumerable<T> _, Action<T> callback)
        {
            foreach (T t in _) callback(t);
        }

        public static bool IsEmpty(this string[] _) => _ == null || _.Length == 0 || _.All(IsNullOrWhiteSpace);
    }
}
