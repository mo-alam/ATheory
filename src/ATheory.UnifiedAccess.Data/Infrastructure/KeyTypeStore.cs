﻿/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System.Collections.Generic;
using System.Linq;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    public class KeyTypeStore
    {
        internal string Container;
        internal string[] Keys;
        internal IDictionary<SpecialKey, List<string>> SpecialKeys;
        internal bool HasSpecialKeys => SpecialKeys != null && SpecialKeys.Count > 0;

        internal void AddSpecialKey(SpecialKey key, string keyName) {
            if (SpecialKeys == null) SpecialKeys = new Dictionary<SpecialKey, List<string>>();
            if (!SpecialKeys.ContainsKey(key)) SpecialKeys.Add(key, new List<string>());
            SpecialKeys[key].Add(keyName);
        }

        internal string GetFirstSpecialKey(SpecialKey key)
        {
            if (!SpecialKeys.ContainsKey(key)) return string.Empty;
            return SpecialKeys[key].FirstOrDefault();
        }
    }
}