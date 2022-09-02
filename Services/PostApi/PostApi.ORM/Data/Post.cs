using System;
using System.Collections.Generic;

namespace PostApi.ORM.Data
{
    public partial class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Source { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RatingCount { get; set; }
        public int CategoryId { get; set; }
    }
}
