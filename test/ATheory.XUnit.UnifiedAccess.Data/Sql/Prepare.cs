using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Infrastructure;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
{
    public static class Prepare
    {
        static bool _preped;

        public static bool Prepared => _preped;
        static Prepare() {
            EntityUnifier.Factory()
                /* Use defualt context */
                .UseDefaultContext(Connection.CreateSqlServer(
                    "LAPTOP-PKM3LA5L\\SQLEXPRESS", 
                    "test-db", 
                    "sa", 
                    "dumb@admin"))
                .Register<Author>();
            _preped = true;
        }

        public static IReadQuery<Author> GetQuery() => EntityUnifier.Factory().GetQueryService<IReadQuery<Author>>();
        public static IWriteQuery<Author> SetQuery() => EntityUnifier.Factory().GetQueryService<IWriteQuery<Author>>();
        public static ISqlQuery SqlQuery() => EntityUnifier.Factory().GetQueryService<ISqlQuery>();
    }
}
