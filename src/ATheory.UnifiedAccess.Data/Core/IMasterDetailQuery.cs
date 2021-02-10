/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
namespace ATheory.UnifiedAccess.Data.Core
{
    /// <summary>
    /// Interface to access functions related master-detail data fetch
    /// </summary>
    /// <typeparam name="TSource">Type of Model</typeparam>
    public interface IMasterDetailQuery<TSource> : IQueryService
        where TSource : class, new()
    {
    }
}