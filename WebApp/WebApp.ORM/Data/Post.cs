using System;
using System.Collections.Generic;

namespace WebPage.ORM.Data
{
    public partial class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Source { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public int RatingCount { get; set; }
        public int CategoryId { get; set; }
    }
}
