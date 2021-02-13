/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class ExprQuerySchemaExt
    {
        #region Public methods

        public static bool CreateSchema<T>(this ISchemaQuery<T> _)
            where T : class, new()
        {
            Error.Clear();
            try
            {
                using var _context = GetContext();
                return _context.CreateSchema<T>();
            }
            catch (Exception e)
            {
                Error.SetContext(e);
                return default;
            }
        }

        #endregion
    }
}
