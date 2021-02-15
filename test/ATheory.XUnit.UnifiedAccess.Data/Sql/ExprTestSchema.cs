using ATheory.UnifiedAccess.Data.Core;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
{
    public class ExprTestSchema
    {
        [Fact]
        public void Create_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.CreateSchema();
            Assert.True(result);
        }

        [Fact]
        public void Alter_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.UpdateSchema();
            Assert.True(result);
        }

        [Fact]
        public void Drop_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.DeleteSchema();
            Assert.True(result);
        }
    }
}