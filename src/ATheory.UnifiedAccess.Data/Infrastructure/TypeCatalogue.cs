/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    public static class TypeCatalogue
    {
        /// <summary>
        /// Type of provider
        /// </summary>
        public enum StorageProvider
        {
            SqlServer,          /* Microsoft SQL Server/SQL Express */
            SqlLite,            /* Microsoft SQLLite */
            Cosmos,             /* Azure Cosmos Db */
            MySql,              /* MySQL database */
            Mongo,              /* Mongo DB */
            PostgreSql,         /* Postgre Sql*/
            DynamoDb,           /* Amazon DynamoDb */
            NotDefined = 99,    /* Provider is not known, context is created by the user */
        }

        /// <summary>
        /// Error type thrown from various areas of the framework
        /// </summary>
        public enum ErrorOrigin
        {
            None,           /* No Error */
            Context,        /* Exception thrown from the underlying DbContext */
            SqlRawSource,   /* Exception thrown from the SQL */
            Schema          /* Exception thrown from the Table/Column schema processor */
        }

        public enum LifeCycle
        {
            TransientPerAction, /* Creates instance every time an operation is requested */
            SingleInstance      /* Creates one instance for a registration */
        }

        public enum SpecialKey {
            PartitionKey,       /* PartitionKey = {CosmosDB, DynamoDB : not requred}*/
            SortKey             /* SortKey = {DynamoDB : not requred}*/
        }

        // Name is exact copy of Amazon's RegionEndpoint
        public enum AmazonRegion
        {
            USEast1,
            MESouth1,
            CACentral1,
            CNNorthWest1,
            CNNorth1,
            USGovCloudWest1,
            USGovCloudEast1,
            APSoutheast2,
            APSoutheast1,
            APSouth1,
            APNortheast3,
            SAEast1,
            APNortheast1,
            APNortheast2,
            USWest1,
            USWest2,
            EUNorth1,
            EUWest1,
            USEast2,
            EUWest3,
            EUCentral1,
            APEast1,
            EUWest2
        }

        internal const string NoContainer = "ATHEORY_CONTAINER_NOCONTAINER";
    }
}
