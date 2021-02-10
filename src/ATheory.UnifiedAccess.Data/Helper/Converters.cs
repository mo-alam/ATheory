/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Text.RegularExpressions;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Helper
{
    public static class Converters
    {
        #region Private method

        // GetAmazonSystemName version is way slower than the GetAmazonSystemNameEx version
        // 10000000 iteration: GetAmazonSystemName = 6598 ms; GetAmazonSystemNameEx = 1099 ms
        // 1000000 iteration: GetAmazonSystemName = 1119 ms; GetAmazonSystemNameEx = 122 ms
        static string GetAmazonSystemName(string raw)
        {
            var name = raw;
            var digit = Regex.Match(name, @"\d+").Value;
            return $"{name.Substring(0, 2)}-{name.Substring(2, name.Length - 2 - digit.Length)}-{digit}";
        }

        static string GetAmazonSystemNameEx(string raw)
        {
            var array = new char[raw.Length + 2];
            int j = 0, i = 0, k = raw.Length - 1;
            int c = k + 2;
            var check = true;
            while (true)
            {
                array[j++] = raw[i++];
                if (j == 2) array[j++] = '-';
                if (check)
                {
                    if (Char.IsDigit(raw[k])) array[c--] = raw[k--];
                    else
                    {
                        check = false;
                        array[c--] = '-';
                        array[c--] = raw[k--];
                    }
                }
                else array[c--] = raw[k--];
                if (i > k) break;
            }

            return new string(array);
        }

        #endregion

        #region Public method

        public static Amazon.RegionEndpoint GetAmazonRegion(this AmazonRegion _)
            => Amazon.RegionEndpoint.GetBySystemName(GetAmazonSystemNameEx(_.ToString()));
        public static Amazon.RegionEndpoint GetAmazonRegion(this object _) => GetAmazonRegion((AmazonRegion)_);
        public static DynamoDBEntry ToEntry(this object _) => new Primitive { Value = _ };

        #endregion
    }
}
