using System;
namespace PostApi.INFRASTRUCTURE.Requests
{
	public class CreateRatingRequest
	{
		// this is our model requesting page.

		public int Id { get; set; }
		public int RatingCount { get; set; }
		public int PostId { get; set; }
	}
}

