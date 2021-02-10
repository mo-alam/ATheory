using System;

namespace ATheory.XUnit.UnifiedAccess.Data.Cosmos
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PartitionKey { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public string id { get; set; }  /* internal property */
    }
}
