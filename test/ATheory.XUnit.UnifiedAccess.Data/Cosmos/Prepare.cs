using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using ATheory.XUnit.UnifiedAccess.Data.Common;

namespace ATheory.XUnit.UnifiedAccess.Data.Cosmos
{
    public static class Prepare
    {
        static bool _preped;

        public static bool Prepared => _preped;
        static Prepare() {
            var config = new JsonShell<ConnConfig>().Load(@"C:\Dev\Configs\cosmos.json");
            EntityUnifier.Factory()
                .UseDefaultContext(Connection.CreateCosmos(config.Key1, config.Key2, "BookDB"))
                .Register<Author>("Author", o => o.Id)
                .SpecialKey("PartitionKey", TypeCatalogue.SpecialKey.PartitionKey)
                .Register<Book>("Books", o => o.Id)
                .SpecialKey("PartitionKey", TypeCatalogue.SpecialKey.PartitionKey);
            _preped = true;
        }

        public static IReadQuery<Author> GetQuery() => EntityUnifier.Factory().GetQueryService<IReadQuery<Author>>();
        public static IWriteQuery<Author> SetQuery() => EntityUnifier.Factory().GetQueryService<IWriteQuery<Author>>();
        public static ISqlQuery SqlQuery() => EntityUnifier.Factory().GetQueryService<ISqlQuery>();
        public static ISchemaQuery<Book> SchemaQuery() => EntityUnifier.Factory().GetQueryService<ISchemaQuery<Book>>();
    }
}
