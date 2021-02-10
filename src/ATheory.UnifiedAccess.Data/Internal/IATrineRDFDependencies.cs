/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace ATheory.UnifiedAccess.Data.Internal
{
    /// <summary>
    /// Interface to hide the internal EF Core interfaces and expose only the public APIs.
    /// </summary>
    internal interface IATrineRDFDependencies 
    {
        IDisposable CriticalSection { get; }
        IRawSqlCommandBuilder CommandBuilder { get; }
    }
}
