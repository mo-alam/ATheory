/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using ATheory.Util.Reflect;

#pragma warning disable EF1001 // Internal EF Core API usage.
namespace ATheory.UnifiedAccess.Data.Internal
{
    internal class ATrineDbFacadeDependencies : IATrineDbFacadeDependencies
    {
        #region Constructor
        internal ATrineDbFacadeDependencies(IDatabaseFacadeDependencies instance)
            => dependencies = instance;

        #endregion


        #region Private members

        IDatabaseFacadeDependencies dependencies;

        #endregion

        #region Implement interface

        T IATrineDbFacadeDependencies.GetDatabaseCreator<T>()
            => dependencies.DatabaseCreator as T;

        bool IATrineDbFacadeDependencies.CreateCosmosContainerIfNotExists(string containerName, string partitionKey)
        {
            if( dependencies.DatabaseCreator is CosmosDatabaseCreator creator)
            {
                var _cosmosClient = Reflector.GetMemberVariable<CosmosClientWrapper>(creator, "_cosmosClient");
                return _cosmosClient.CreateContainerIfNotExists(containerName, partitionKey);
            }
            return false;
        }

        #endregion
    }
}

#pragma warning restore EF1001 // Internal EF Core API usage.
