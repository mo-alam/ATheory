using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ATheory.XUnit.UnifiedAccess.Data.Mongo
{
    public class Author
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
