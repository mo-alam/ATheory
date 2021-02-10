using ATheory.UnifiedAccess.Data.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Cosmos
{
    public class ExprTestWrite
    {
        [Fact]
        public void Insert_Delete_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Insert(
                new Author { Id = 99, Name = "James Patterson", PartitionKey = "99", Description = "Crime - Thrillier", Index = 99 });

            Assert.True(result);

            result = query.Delete(a => a.Id == 99);

            Assert.True(result);
        }

        [Fact]
        public void Update_all_one_success()
        {
            var query = Prepare.SetQuery();

            var allProperty = Prepare.GetQuery().GetFirst(a => a.Id == 7);
            allProperty.Index = DateTime.Now.Day;
            allProperty.Description = $"Updated on {DateTime.Now}";

            var result = query.Update(allProperty);

            Assert.True(result);

            var oneProperty = Prepare.GetQuery().GetFirst(a => a.Id == 9);
            oneProperty.Index = DateTime.Now.Day;
            oneProperty.Description = $"Updated on {DateTime.Now}";

            result = query.Update(oneProperty, p => p.Description);

            Assert.True(result);
        }

        [Fact]
        public void InsertBulk_success()
        {
            // Support for Bulk insert will come later

            //var query = Prepare.SetQuery();
            //var newList = new List<Author> { 
            //    new Author {
            //        Id = 1,
            //        PartitionKey = "1",
            //        Name="Frank Herbert",
            //        index = DateTime.Now.Day,
            //        description ="SciFi"
            //    },
            //    new Author {
            //        index = DateTime.Now.Day,
            //        description = $"Inserted (2) on {DateTime.Now}"
            //        //datemod = DateTime.Now,
            //        //amount = DateTime.Now.DayOfYear
            //    }
            //};
            //var result = query.InsertBulk(newList);
            //Assert.True(result);
        }

        [Fact]
        public void PrepareData()
        {
            var query = Prepare.SetQuery();
            var newList = new List<Author> {
                new Author {
                    Id = 1,
                    PartitionKey = "1",
                    Name="Frank Herbert",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 2,
                    PartitionKey = "1",
                    Name="Isaac Asimov",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 3,
                    PartitionKey = "1",
                    Name="Robert A Heinlein",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 4,
                    PartitionKey = "1",
                    Name="Rebert Silverberg",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 5,
                    PartitionKey = "1",
                    Name="Greg Bear",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 6,
                    PartitionKey = "1",
                    Name="Greg Bear",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 7,
                    PartitionKey = "1",
                    Name="Frank Herbert",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 8,
                    PartitionKey = "1",
                    Name="Isaac Asimov",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 9,
                    PartitionKey = "1",
                    Name="Robert A Heinlein",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 10,
                    PartitionKey = "11",
                    Name="Rebert Silverberg",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                },
                new Author {
                    Id = 11,
                    PartitionKey = "11",
                    Name="Greg Bear",
                    Index = DateTime.Now.Day,
                    Description ="SciFi"
                }
            };
            foreach (var author in newList) query.Insert(author);
        }
    }
}
