using ATheory.UnifiedAccess.Data.Core;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Dynamo
{
    public class ExprTestSchema
    {
        [Fact]
        public void Create_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.CreateSchema<Novel>();
            Assert.True(result);
        }

        [Fact]
        public void Delete_success()
        {
            var query = Prepare.SchemaQuery();
            var result = query.DeleteSchema<Novel>();
            Assert.True(result);
        }
    }
}