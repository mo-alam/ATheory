using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATheory.XUnit.UnifiedAccess.Data.Bridge
{
    [Table("author", Schema ="dbo")]
    public class Author
    {
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public string description { get; set; }

        public DateTime? datemod { get; set; }
        public decimal? amount { get; set; }
        public int index { get; set; }
    }

    public class AuthorMongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public int __v { get; set; }  /* internal property */
    }
}
