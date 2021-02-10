using ATheory.UnifiedAccess.Data.Core;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Dynamo
{
    public class ExprTestRead
    {
        [Fact]
        public void GetFirst_not_null_value_match()
        {
            var query = Prepare.GetBooks();
            var result = query.GetFirst(a => a.PartKey == 1 /*&& a.Id == "3"*/);

            Assert.NotNull(result);
            //Assert.Equal(3, result.Index);
        }

        // Commenting it out, there is bug in the ef core with LastOrDefault
        //[Fact]
        //public void GetLast_not_null_value_match()
        //{
        //    var query = Prepare.GetQuery();
        //    var result = query.GetLast(a => a.Index < 6);

        //    Assert.NotNull(result);
        //    Assert.Equal(3, result.Index);
        //}

        [Fact]
        public void GetTop_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetTop(a => a.Index >= 5 && a.Index < 12, k => k.Index);

            Assert.NotNull(result);
            Assert.Equal(11, result.Index);
        }

        [Fact]
        public void GetBottom_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetBottom(a => a.Index >= 5, k => k.Index);

            Assert.NotNull(result);
            Assert.Equal(5, result.Index);
        }

        [Fact]
        public void GetList_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetList();
            var notall = query.GetList(a => a.Name.Contains("Isaac Asimov"));

            Assert.NotNull(all);
            Assert.True(all.Count >= 11);

            Assert.NotNull(notall);
            Assert.Equal(2, notall.Count);
        }

        [Fact]
        public void GetOrderedList_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetOrderedList(k => k.Name);
            var notall = query.GetOrderedList(k => k.Name, a => a.Index > 0 && a.Index <= 6);

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
            var notall = query.GetDescendingOrderedList(k => k.Name, a => a.Index > 0 && a.Index <= 6);

            Assert.NotNull(all);
            Assert.True(all.Count >= 10);

            Assert.NotNull(notall);
            Assert.Equal(6, notall.Count);
        }

        [Fact]
        public void GetRange_not_null_count()
        {
            var query = Prepare.GetQuery();
            var all = query.GetRange((2, 3));
            var notall = query.GetRange((2, 3), a => a.Index > 0 && a.Index <= 6);

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
            var notall = query.GetRangeOrderBy((2, 3), k => k.Id, a => a.Index > 0 && a.Index <= 6);

            Assert.NotNull(all);
            Assert.Equal(3, all.Count);

            Assert.NotNull(notall);
            Assert.Equal(3, notall.Count);
        }
    }
}
