/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using static System.String;

namespace ATheory.Util.Extensions
{
    public static class Tools
    {
        #region Public methods

        public static string ToMessage(this Exception _)
        {
            var exception = _;
            var msg = Empty;
            do
            {
                msg += $"Exception [{exception.GetType().Name}], Message = {exception.Message} \r\n";
                exception = exception.InnerException;
            } while (exception != null);

            return msg;
        } 

        #endregion
    }
}
