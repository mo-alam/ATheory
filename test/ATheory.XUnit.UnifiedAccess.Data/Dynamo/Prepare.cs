using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;
using ATheory.Util.Extensions;
using ATheory.XUnit.UnifiedAccess.Data.Common;

namespace ATheory.XUnit.UnifiedAccess.Data.Dynamo
{
    public static class Prepare
    {
        static bool _preped;

        public static bool Prepared => _preped;
        static Prepare() {
            var config = new JsonShell<ConnConfig>().Load(@"C:\Dev\Configs\dynamo.json");
            EntityUnifier.Factory()
                .UseDefaultContext(Connection.CreateDynamo(config.Key1, config.Key2, TypeCatalogue.AmazonRegion.USEast2))
                .Register<Author>(collectionName: "Authors")
                .Register<Books>(collectionName: "Books");
            _preped = true;
        }

        public static IReadQuery<Author> GetQuery() => EntityUnifier.Factory().GetQueryService<IReadQuery<Author>>();
        public static IWriteQuery<Author> SetQuery() => EntityUnifier.Factory().GetQueryService<IWriteQuery<Author>>();
        public static IReadQuery<Books> GetBooks() => EntityUnifier.Factory().GetQueryService<IReadQuery<Books>>();
    }
}
