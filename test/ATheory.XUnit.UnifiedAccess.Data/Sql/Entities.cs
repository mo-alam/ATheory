﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATheory.XUnit.UnifiedAccess.Data.Sql
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

    public class AuthorObj
    {
        public int Id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public DateTime datemod { get; set; }
    }

    public class AuthorDto
    {
        public int Id { get; set; }

        public string name { get; set; }
    }

    [Table("Books")]
    public class Book
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [Column("Published")]
        public DateTime PublishedDate { get; set; }

        [MaxLength(355)]
        public string Description { get; set; }

        public bool IsInPrint { get; set; }
        public Int64 LongValue { get; set; }
    }
}
