/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.Util.Extensions;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    public class Connection
    {
        #region Constructor

        public Connection() { }
        Connection(StorageProvider provider, string hostOrServer, string dataSource, string userName, string password, object userDefined)
        {
            Provider = provider;
            switch (Provider)
            {
                case StorageProvider.SqlServer:
                    ConnectionString = $"data source={hostOrServer};initial catalog={dataSource};persist security info=True;user id={userName};password={password};pooling=False;MultipleActiveResultSets=True;";
                    break;
                case StorageProvider.Cosmos:
                    EndPoint = hostOrServer;
                    Database = dataSource;
                    Key = userName;
                    break;
                case StorageProvider.SqlLite:
                    ConnectionString = $"Data Source={hostOrServer};Version=3;Password={password};";
                    break;
                case StorageProvider.MySql:
                    ConnectionString = $"server={hostOrServer};database={dataSource};user={userName};password={password}";
                    break;
                case StorageProvider.Mongo:
                    var port = userDefined.ToInt(27017);
                    if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                        ConnectionString = $"mongodb://{userName}:{password}@{hostOrServer}:{port}";
                    else
                        ConnectionString = $"mongodb://{hostOrServer}:{port}";
                    Database = dataSource;
                    break;
                case StorageProvider.DynamoDb:
                    Key = hostOrServer;
                    EndPoint = dataSource;
                    UserDefined = userDefined; 
                    break;
            }
        }

        #endregion
                
        #region Properties

        public StorageProvider Provider { get; }
        public string ConnectionString { get; }
        // Secret Key for DynamoDb
        public string EndPoint { get; }
        public string Key { get; }
        public string Database { get; }
        public object UserDefined { get; }

        #endregion

        #region Static methods

        public static Connection CreateSqlServer(string server, string database, string userName, string password) => 
            new Connection(StorageProvider.SqlServer, server, database, userName, password, 0);

        public static Connection CreateSqlLite(string databasePath, string password) =>
            new Connection(StorageProvider.SqlLite, databasePath, string.Empty, string.Empty, password, 0);

        public static Connection CreateCosmos(string endpoint, string accountKey, string database) =>
            new Connection(StorageProvider.Cosmos, endpoint, database, accountKey, string.Empty, 0);

        public static Connection CreateMongo(string host, string defaultDatabase, int port = 0) =>
            new Connection(StorageProvider.Mongo, host, defaultDatabase, string.Empty, string.Empty, port == 0 ? null : (object)port);

        public static Connection CreateMongo(string host, string defaultDatabase, string userName, string password, int port = 0) =>
            new Connection(StorageProvider.Mongo, host, defaultDatabase, userName, password, port);

        public static Connection CreateDynamo(string accessKey, string secretAccessKey, AmazonRegion region) =>
            new Connection(StorageProvider.DynamoDb, accessKey, secretAccessKey, string.Empty, string.Empty, region);

        #endregion
    }
}