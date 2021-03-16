using ATheory.UnifiedAccess.Data.Core;
using System.Linq;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Cosmos
{
    public class ExprTestRead
    {
        [Fact]
        public void GetFirst_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetFirst(a => a.Id == 88);
            Assert.NotNull(result);
            Assert.Equal(88, result.Id);
        }
        
        [Fact]
        public void GetLast_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetLast(a => a.Id < 6);
            
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }
        

        [Fact]
        public void GetTop_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetTop(a => a.Id >= 5 && a.Id < 12, k => k.Id);
            
            Assert.NotNull(result);
            Assert.Equal(11, result.Id);
        }

        [Fact]
        public void GetBottom_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetBottom(a => a.Id >= 5, k => k.Id);
            
            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
        }

        [Fact]
        public void GetList_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetList();
            var notall = query.GetList(a => a.Id > 0 && a.Id <= 6);

            Assert.NotNull(all);
            Assert.True(all.Count >= 10);

            Assert.NotNull(notall);
            Assert.Equal(5, notall.Count);
        }

        [Fact]
        public void GetOrderedList_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetOrderedList(k => k.Name);
            var notall = query.GetOrderedList(k => k.Name, a => a.Id > 0 && a.Id <= 6);

            Assert.NotNull(all);
            Assert.True(all.Count >= 10);

            Assert.NotNull(notall);
            Assert.Equal(6, notall.Count);

        }

        [Fact]
        public void GetDescendingOrderedList_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetDescendingOrderedList(k => k.Name);
            var notall = query.GetDescendingOrderedList(k => k.Name, a => a.Id > 0 && a.Id <= 6);

            Assert.NotNull(all);
            Assert.True(all.Count >= 10);

            Assert.NotNull(notall);
            Assert.Equal(5, notall.Count);
        }

        [Fact]
        public void GetRange_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetRange((2, 3));
            var notall = query.GetRange((2, 3), a => a.Id > 0 && a.Id <= 6);

            Assert.NotNull(all);
            Assert.Equal(3, all.Count);

            Assert.NotNull(notall);
            Assert.Equal(3, notall.Count);
        }

        [Fact]
        public void GetRangeOrderBy_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetRangeOrderBy((0, 3), k => k.Id);
            var notall = query.GetRangeOrderBy((2, 3), k => k.Id, a => a.Id > 0 && a.Id <= 6);

            Assert.NotNull(all);
            Assert.Equal(3, all.Count);

            Assert.NotNull(notall);
            Assert.Equal(3, notall.Count);
        }
    }
}
