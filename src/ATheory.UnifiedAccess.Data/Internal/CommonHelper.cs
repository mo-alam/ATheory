/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Internal
{
    internal static class CommonHelper
    {
        /// <summary>
        /// Execute a function in a try catch block to avoid repeating the same code again and again
        /// </summary>
        /// <typeparam name="T">Return type of the function</typeparam>
        /// <param name="func">The function that'll be run inside the try-catch</param>
        /// <returns>The result of function or the default in case of failure</returns>
        internal static T ExecGuarded<T>(Func<T> func)
        {
            Error.Clear();
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return default;
            }
        }
    }
}
