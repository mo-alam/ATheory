using ATheory.UnifiedAccess.Data.Core;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
{
    public class SqlExprTestRead
    {
        [Fact]
        public void GetFirst_not_null_value_match_dto()
        {
            var query = Prepare.GetQuery();
            var result = query.GetFirst(a => a.Id > 2);
            var dto = query.GetFirst(a => a.Id > 2, s => new AuthorDto { Id = s.Id, name = s.Name });

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);

            Assert.NotNull(dto);
            Assert.Equal(3, dto.Id);
        }
        
        // Commenting it out, there is bug in the ef core with LastOrDefault
        /*
        [Fact]
        public void GetLast_not_null_value_match()
        {
            var query = Prepare.GetQuery();
            var result = query.GetLast(a => a.Id > 2);
            var dto = query.GetLast(a => a.Id > 2, s => new AuthorDto { Id = s.Id, name = s.Name });

            Assert.NotNull(result);
            Assert.Equal(12, result.Id);

            Assert.NotNull(dto);
            Assert.Equal(12, dto.Id);
        }
        */

        [Fact]
        public void GetTop_not_null_value_match_dto()
        {
            var query = Prepare.GetQuery();
            var result = query.GetTop(a => a.Id >= 5 && a.Id < 12, k => k.Id);
            var dto = query.GetTop(a => a.Id > 5 && a.Id < 12, k => k.Id, s => new AuthorDto { Id = s.Id, name = s.Name });

            Assert.NotNull(result);
            Assert.Equal(11, result.Id);

            Assert.NotNull(dto);
            Assert.Equal(11, dto.Id);
        }

        [Fact]
        public void GetBottom_not_null_value_match_dto()
        {
            var query = Prepare.GetQuery();
            var result = query.GetBottom(a => a.Id >= 5, k => k.Id);
            var dto = query.GetBottom(a => a.Id >= 5, k => k.Id, s => new AuthorDto { Id = s.Id, name = s.Name });

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);

            Assert.NotNull(dto);
            Assert.Equal(5, dto.Id);
        }

        [Fact]
        public void GetList_not_null_count_dto()
        {
            var query = Prepare.GetQuery();
            var all = query.GetList();
            var notall = query.GetList(a => a.index > 0 && a.index <= 11);
            var dto = query.GetList(s => new AuthorDto { Id = s.Id, name = s.Name }, a => a.index > 0 && a.index <= 11);

            Assert.NotNull(all);
            Assert.True(all.Count > 10);

            Assert.NotNull(notall);
            Assert.Equal(6, notall.Count);

            Assert.NotNull(dto);
            Assert.Equal(6, dto.Count);
        }

        [Fact]
        public void GetOrderedList_not_null_count_dto()
        {
            var query = Prepare.GetQuery();
            var all = query.GetOrderedList(k => k.Name);
            var notall = query.GetOrderedList(k => k.Name, a => a.index > 0 && a.index <= 11);
            var dto = query.GetOrderedList(k => k.Name, s => new AuthorDto { Id = s.Id, name = s.Name }, a => a.index > 0 && a.index <= 11);

            Assert.NotNull(all);
            Assert.True(all.Count > 10);

            Assert.NotNull(notall);
            Assert.Equal(6, notall.Count);

            Assert.NotNull(dto);
            Assert.Equal(6, dto.Count);
        }

        [Fact]
        public void GetDescendingOrderedList_not_null_count_dto()
        {
            var query = Prepare.GetQuery();
            var all = query.GetDescendingOrderedList(k => k.Name);
            var notall = query.GetDescendingOrderedList(k => k.Name, a => a.index > 0 && a.index <= 11);
            var dto = query.GetDescendingOrderedList(k => k.Name, s => new AuthorDto { Id = s.Id, name = s.Name }, a => a.index > 0 && a.index <= 11);

            Assert.NotNull(all);
            Assert.True(all.Count > 10);

            Assert.NotNull(notall);
            Assert.Equal(6, notall.Count);

            Assert.NotNull(dto);
            Assert.Equal(6, dto.Count);
        }

        [Fact]
        public void GetRange_not_null_count_dto()
        {
            var query = Prepare.GetQuery();
            var all = query.GetRange((2, 3));
            var notall = query.GetRange((2, 3), a => a.index > 0 && a.index <= 11);
            var dto = query.GetRange((2, 3), s => new AuthorDto { Id = s.Id, name = s.Name }, a => a.index > 0 && a.index <= 11);

            Assert.NotNull(all);
            Assert.Equal(3, all.Count);

            Assert.NotNull(notall);
            Assert.Equal(3, notall.Count);

            Assert.NotNull(dto);
            Assert.Equal(3, dto.Count);
        }

        [Fact]
        public void GetRangeOrderBy_not_null_count_dto()
        {
            var query = Prepare.GetQuery();
            var all = query.GetRangeOrderBy((0, 3), k => k.Id);
            var notall = query.GetRangeOrderBy((2, 3), k => k.Id, a => a.index > 0 && a.index <= 11);
            var dto = query.GetRangeOrderBy((2, 3), k => k.Id, s => new AuthorDto { Id = s.Id, name = s.Name }, a => a.index > 0 && a.index <= 11);

            Assert.NotNull(all);
            Assert.Equal(3, all.Count);

            Assert.NotNull(notall);
            Assert.Equal(3, notall.Count);

            Assert.NotNull(dto);
            Assert.Equal(3, dto.Count);
        }
    }
}
