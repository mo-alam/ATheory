namespace ATheory.XUnit.UnifiedAccess.Data.Dynamo
{
    public class Author
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public string NewProp { get; set; }
    }

    public class Books
    {
        public int PartKey { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Novel
    {
        public int PartKey { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
