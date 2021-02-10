using ATheory.UnifiedAccess.Data.Core;
using ATheory.UnifiedAccess.Data.Sql;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
{
    public class RawSqlTest
    {
        [Fact]
        public void GetFirst_not_null_value_match()
        {
            var query = Prepare.SqlQuery();
            var result = query.GetFirst<AuthorObj>("select Id, name from author where Id > 5");

            Assert.NotNull(result);
            Assert.Equal(6, result.Id);
        }

        [Fact]
        public void GetLast_not_null_value_match()
        {
            var query = Prepare.SqlQuery();
            var result = query.GetLast<AuthorObj>("select Id, name from author where Id < 12");

            Assert.NotNull(result);
            Assert.Equal(11, result.Id);
        }

        [Fact]
        public void GetList_not_null_count()
        {
            var query = Prepare.SqlQuery();
            var result = query.GetList<AuthorObj>("select Id, name from author where Id >= 3 and Id <= 12");

            Assert.NotNull(result);
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void Insert_value_success()
        {
            var query = Prepare.SqlQuery();
            var result = query.Execute("insert into author (name, description) values (@name, @description)", SqlHelper.Parameters.Get("@name","Isaac"), SqlHelper.Parameters.Get("@description", "sci-fi"));

            Assert.True(result);
        }
    }
}