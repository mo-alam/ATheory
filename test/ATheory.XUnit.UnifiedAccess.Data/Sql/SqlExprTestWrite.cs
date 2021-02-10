using ATheory.UnifiedAccess.Data.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
{
    public class SqlExprTestWrite
    {
        [Fact]
        public void Insert_Delete_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Insert(new Author { index = 99, Name = "New insert", amount = 88, datemod = DateTime.Now });

            Assert.True(result);

            result = query.Delete(a => a.index == 99);
            Assert.True(result);
        }

        [Fact]
        public void Update_all_one_success()
        {
            var query = Prepare.SetQuery();

            var allProperty = Prepare.GetQuery().GetFirst(a => a.Id == 7);
            allProperty.index = DateTime.Now.Day;
            allProperty.description = $"Updated on {DateTime.Now}";
            allProperty.datemod = DateTime.Now;
            allProperty.amount = DateTime.Now.DayOfYear;

            var result = query.Update(allProperty);

            Assert.True(result);

            var oneProperty = Prepare.GetQuery().GetFirst(a => a.Id == 9);
            oneProperty.index = DateTime.Now.Day;
            oneProperty.description = $"Updated on {DateTime.Now}";
            oneProperty.datemod = DateTime.Now;
            oneProperty.amount = DateTime.Now.DayOfYear;

            result = query.Update(oneProperty, p => p.description);

            Assert.True(result);
        }

        [Fact]
        public void InsertBulk_success()
        {
            var query = Prepare.SetQuery();
            var newList = new List<Author> { 
                new Author {
                    index = DateTime.Now.Day,
                    description = $"Inserted (1) on {DateTime.Now}",
                    datemod = DateTime.Now,
                    amount = DateTime.Now.DayOfYear
                },
                new Author {
                    index = DateTime.Now.Day,
                    description = $"Inserted (2) on {DateTime.Now}",
                    datemod = DateTime.Now,
                    amount = DateTime.Now.DayOfYear
                }
            };
            var result = query.InsertBulk(newList);
            Assert.True(result);
        }
    }
}
