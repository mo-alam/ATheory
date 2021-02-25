using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using ATheory.XUnit.UnifiedAccess.Data.Common;
using System;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
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
                .UseDefaultContext(Connection.CreateSqlServer(config.Key1, config.Key2, config.Key3, config.Password))
                .Register<Author>()
                .Register<Book>(b => b.Id);
            _preped = true;
        }

        public static IReadQuery<Author> GetQuery() => EntityUnifier.Factory().GetQueryService<IReadQuery<Author>>();
        public static IWriteQuery<Author> SetQuery() => EntityUnifier.Factory().GetQueryService<IWriteQuery<Author>>();
        public static ISqlQuery SqlQuery() => EntityUnifier.Factory().GetQueryService<ISqlQuery>();
        public static ISchemaQuery<Book> SchemaQuery() => EntityUnifier.Factory().GetQueryService<ISchemaQuery<Book>>();
    }
}
