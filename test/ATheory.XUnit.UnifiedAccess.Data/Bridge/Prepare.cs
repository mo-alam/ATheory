using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using ATheory.XUnit.UnifiedAccess.Data.Common;
using System;

namespace ATheory.XUnit.UnifiedAccess.Data.Bridge
{
    public static class Prepare
    {
        static bool _preped;

        public static bool Prepared => _preped;

        static Prepare()
        {
            var config = new JsonShell<ConnConfig>().Load(@"C:\Dev\Configs\sql.json");
            EntityUnifier.Factory()
                /* Use defualt context */
                .UseDefaultContext(Connection.CreateSqlServer(config.Key1, config.Key2, config.Key3, config.Password),"sql-context")
                .Register<Author>()
                .UseDefaultContext(Connection.CreateMongo("localhost", "bookdb"),"mongo-context")
                .Register<AuthorMongo>(collectionName: "Authors");
            _preped = true;
        }

        public static IBridge GetBridge() => EntityUnifier.Factory().Bridge("sql-context", "mongo-context");
    }
}
