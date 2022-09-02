using System;
namespace WebPage.DTO
{
	public class NotificationDTO
	{
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Notifications { get; set; }
        public int PinnedVideosId { get; set; }
        public int FollowerId { get; set; }
        public int FollowedUserId { get; set; }
        public int RatingId { get; set; }
    }
}

