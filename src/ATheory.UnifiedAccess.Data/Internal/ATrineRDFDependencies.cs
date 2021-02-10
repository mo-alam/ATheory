/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace ATheory.UnifiedAccess.Data.Internal
{
    internal class ATrineRDFDependencies : IATrineRDFDependencies
    {
        #region Constructor

        internal ATrineRDFDependencies(IRelationalDatabaseFacadeDependencies dependencies) 
            => Instance = dependencies;

        #endregion

        #region Members

        internal IRelationalDatabaseFacadeDependencies Instance;

        #endregion

        #region Implements interface

        IDisposable IATrineRDFDependencies.CriticalSection => Instance.ConcurrencyDetector.EnterCriticalSection();
        IRawSqlCommandBuilder IATrineRDFDependencies.CommandBuilder => Instance.RawSqlCommandBuilder;
        
        #endregion
    }
}
