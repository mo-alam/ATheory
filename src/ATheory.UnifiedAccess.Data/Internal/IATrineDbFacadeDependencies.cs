/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using Microsoft.EntityFrameworkCore.Storage;

namespace ATheory.UnifiedAccess.Data.Internal
{
    /// <summary>
    /// Interface to hide the internal EF Core DBFacade interfaces and expose only the public APIs.
    /// </summary>
    internal interface IATrineDbFacadeDependencies
    {
        /// <summary>
        /// Get the instance of the database creator
        /// </summary>
        /// <typeparam name="T">Type that implements IDatabaseCreator</typeparam>
        /// <returns>Instance of the database creator</returns>
        T GetDatabaseCreator<T>() where T : class, IDatabaseCreator;

        /// <summary>
        /// Create a Cosmos db container in the database if not exists.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns></returns>
        bool CreateCosmosContainerIfNotExists(string containerName, string partitionKey);
    }
}
