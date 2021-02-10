/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ATheory.UnifiedAccess.Data.Context;
using ATheory.UnifiedAccess.Data.Core;
using static ATheory.Util.Reflect.Reflector;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ATheory.UnifiedAccess.Data.Internal
{
    internal static class EFCoreInternalService
    {
        #region Constants

        const string DependencyMethodName = "GetFacadeDependencies";

        #endregion

        #region Methods
        
        internal static DbContext ToEECore(this IUnifiedContext _) => (UnifiedContext)_;
        internal static UnifiedContext ToCore(this IUnifiedContext _) => (UnifiedContext)_;
        internal static DatabaseFacade GetDbFacade(this IUnifiedContext _) => ((UnifiedContext)_).Database;
        internal static IATrineRDFDependencies GetFacadeDependencies(this IUnifiedContext _)
        {
            var instance = InvokeMethod<IRelationalDatabaseFacadeDependencies>(
                  typeof(RelationalDatabaseFacadeExtensions),
                  DependencyMethodName,
                  new object[] { _.GetDbFacade()}
                  );
            return new ATrineRDFDependencies(instance);
        }

        internal static RelationalCommandParameterObject CreateCommandParameterObject(
            this IUnifiedContext _,
            IATrineRDFDependencies dependencies,
            IReadOnlyDictionary<string, object> paramValues)
        {
            var facade = ((ATrineRDFDependencies)dependencies).Instance;
            return new RelationalCommandParameterObject(
                facade.RelationalConnection,
                paramValues,
                null,
                (UnifiedContext)_,
                facade.CommandLogger
                );
        }

        #endregion
    }
}
