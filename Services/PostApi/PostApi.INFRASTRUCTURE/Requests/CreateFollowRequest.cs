using System;
namespace PostApi.INFRASTRUCTURE.Requests
{
	public class CreateFollowRequest
	{
        // this is our model requesting page.

            public int Id { get; set; }
            public int FollowedId { get; set; }
            public int FollowerId { get; set; }
      
    
	}
}

