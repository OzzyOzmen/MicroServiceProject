using System;
namespace PostApi.INFRASTRUCTURE.Requests
{
	public class CreatePinnedPostRequest
	{
		// this is our model requesting page.

		public int Id { get; set; }
		public int UserId { get; set; }
		public int PostId { get; set; }

	}
}

