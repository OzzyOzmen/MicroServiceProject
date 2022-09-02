using System;
namespace PostApi.INFRASTRUCTURE.Requests
{
     public class CreatePostRequest
	{
        // this is our model requesting page.

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Source { get; set; }
        public string Thumbnail { get; set; }
        public string CreatedDate { get; set; }
        public int RatingCount { get; set; }
        public int CategoryId { get; set; }
    }
}

