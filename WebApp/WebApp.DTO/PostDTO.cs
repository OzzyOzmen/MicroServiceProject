using System;

namespace WebPage.DTO
{
	public class PostDTO
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

