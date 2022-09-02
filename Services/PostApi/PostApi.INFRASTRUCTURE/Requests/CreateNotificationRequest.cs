using System;
namespace PostApi.INFRASTRUCTURE.Requests
{
	public class CreateNotificationRequest
	{
        // this is our model requesting page.

        public int Id { get; set; }
        public int PostId { get; set; }
        public string Notifications { get; set; }
        public int PinnedVideosId { get; set; }
        public int FollowerId { get; set; }
        public int FollowedUserId { get; set; }
        public int RatingId { get; set; }
    }
}

