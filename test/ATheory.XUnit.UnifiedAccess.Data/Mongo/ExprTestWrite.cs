using ATheory.UnifiedAccess.Data.Core;
using System;
using System.Collections.Generic;
using Xunit;

namespace ATheory.XUnit.UnifiedAccess.Data.Mongo
{
    public class ExprTestWrite
    {
        [Fact]
        public void Insert_Delete_success()
        {
            var query = Prepare.SetQuery();
            var result = query.Insert(
                new Author { Name = "Isaac Asimov", Description = "Sci-fi", Index = 99 });

            Assert.True(result);

            result = query.Delete(a => a.Index == 99);

            Assert.True(result);
        }

        [Fact]
        public void Update_all_one_success()
        {
            var query = Prepare.SetQuery();
            var newAuthor = Prepare.GetQuery().GetFirst(a => a.Index == 11);
            newAuthor.Description = "Greg Bear changed";
            newAuthor.Name = "James Patterson" ;
            var result = query.Update(a => a.Index == 11, newAuthor);

            Assert.True(result);
        }

        [Fact]
        public void Delete_all_success()
        {
            var query = Prepare.SetQuery();

            var result = query.Delete(a => a.Index < 100);

            Assert.True(result);
        }


        [Fact]
        public void InsertBulk_success()
        {
            var query = Prepare.SetQuery();
            var newList = new List<Author> {
                new Author {
                    Name="Frank Herbert",
                    Index = 1,
                    Description ="SciFi"
                },
                new Author {
                    Name="Isaac Asimov",
                    Index = 2,
                    Description ="SciFi"
                },
                new Author {
                    Name="Robert A Heinlein",
                    Index = 3,
                    Description ="SciFi"
                },
                new Author {                   
                    Name="Rebert Silverberg",
                    Index = 4,
                    Description ="SciFi"
                },
                new Author {
                    Name="Greg Bear",
                    Index = 5,
                    Description ="SciFi"
                },
                new Author {
                    Name="Greg Bear",
                    Index = 6,
                    Description ="SciFi"
                },
                new Author {
                    Name="Frank Herbert",
                    Index = 7,
                    Description ="SciFi"
                },
                new Author {
                    Name="Isaac Asimov",
                    Index = 8,
                    Description ="SciFi"
                },
                new Author {
                    Name="Robert A Heinlein",
                    Index = 9,
                    Description ="SciFi"
                },
                new Author {
                    Name="Rebert Silverberg",
                    Index = 10,
                    Description ="SciFi"
                },
                new Author {
                    Name="Greg Bear",
                    Index = 11,
                    Description ="SciFi"
                }
            };
            var result = query.InsertBulk(newList);
            Assert.True(result);
        }
    }
}
