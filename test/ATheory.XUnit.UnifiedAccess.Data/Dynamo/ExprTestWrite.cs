using ATheory.UnifiedAccess.Data.Core;
using System.Collections.Generic;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Dynamo
{
    public class ExprTestWrite
    {
        [Fact]
        public void Insert_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Insert(new Author { Id = "99", Name = "James Patterson", Index = 99 });

            Assert.True(result);
        }

        [Fact]
        public void InsertBulk_success()
        {
            var query = Prepare.SetQuery();
            var result = query.InsertBulk( new List<Author> {
                new Author { Id = "55", Name = "Greg Bear", Description="Sci-fi", Index = 55 },
                new Author { Id = "66", Name = "Clark", Index = 66 },
                new Author { Id = "77", Name = "Gregory Benford", Description ="Science flick", Index = 77 },
                new Author { Id = "88", Name = "David Drake", Index = 88 },
                new Author { Id = "99", Name = "James Patterson", Index = 99 }
            });

            Assert.True(result);
        }

        [Fact]
        public void Delete_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Delete(a => a.Id == "99");

            Assert.True(result);
        }

        [Fact]
        public void Update_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Update(a => a.Id == "3", new Author { Name = "Peter F. Hamilton", Index = 3 });

            Assert.True(result);
        }

        [Fact]
        public void Update_Props_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Update(new Author { Id = "3", Name = "Hamilton", Index = 8, Description ="Sci-fi", NewProp ="Ultra types" }, e=>e.Description, e=>e.NewProp);

            Assert.True(result);
        }

        [Fact]
        public void Update_NewProperty_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Update(a => a.Id == "3", new Author { NewProp = "80's"});

            Assert.True(result);
        }
    }
}
