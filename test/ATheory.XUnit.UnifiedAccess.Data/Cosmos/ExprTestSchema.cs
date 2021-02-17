using ATheory.UnifiedAccess.Data.Core;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Cosmos
{
    public class ExprTestSchema
    {
        [Fact]
        public void Create_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.CreateSchema<Book>();
            Assert.True(result);
        }

        [Fact]
        public void Delete_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.DeleteSchema<Book>();
            Assert.True(result);
        }
    }
}