/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ATheory.UnifiedAccess.Data.Context
{
    /// <summary>
    /// DbContext for CosmosDb
    /// </summary>
    public class CosmosContext : UnifiedContext
    {
        public CosmosContext(Connection conn) : base(conn)
        {
        }

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
    }
}