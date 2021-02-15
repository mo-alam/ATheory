/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ATheory.UnifiedAccess.Data.Internal;
using static ATheory.UnifiedAccess.Data.Infrastructure.EntityUnifier;

namespace ATheory.UnifiedAccess.Data.Context
{
    /// <summary>
    /// DbContext for CosmosDb
    /// </summary>
    public class CosmosContext : UnifiedContext
    {
        #region Constructor
        public CosmosContext(Connection conn) : base(conn)
        {
        }

        #endregion

        #region Overriden methods

        protected override void RegisterEntity(
            EntityTypeBuilder typeBuilder,
            (string container, KeyTypeStore keyStore) args)
        {
            if (!args.container.IsEmptyOrExact(TypeCatalogue.NoContainer))
                typeBuilder.ToContainer(args.container);

            if (!args.keyStore.Keys.IsEmpty())
                typeBuilder.HasKey(args.keyStore.Keys);
            else
                typeBuilder.HasNoKey();

            if (args.keyStore.SpecialKeys == null) return;
            foreach (var keyType in args.keyStore.SpecialKeys)
            {
                switch (keyType.Key)
                {
                    case TypeCatalogue.SpecialKey.PartitionKey:
                        keyType.Value.ForEach(name => typeBuilder.HasPartitionKey(name));
                        break;
                }
            }
        }

        protected override bool CreateEntitySchema<TEntity>()
        {
            if (!GetRegisteredTypes().ContainsKey(typeof(TEntity))) return false;
            var store = GetRegisteredTypes()[typeof(TEntity)];

            var dependencies = this.GetDbFacadeDependencies();
            return dependencies.CreateCosmosContainerIfNotExists(
                store.container, 
                store.keyStore.GetFirstSpecialKey(TypeCatalogue.SpecialKey.PartitionKey));
        }

        #endregion
    }
}