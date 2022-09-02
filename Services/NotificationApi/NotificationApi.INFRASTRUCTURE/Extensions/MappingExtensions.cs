using System;
using NotificationApi.DTO;
using NotificationApi.ORM.Data;

namespace NotificationApi.INFRASTRUCTURE.Extensions
{
	public static class MappingExtensions
	{
        // Here is our mapping page to DTO's


        public static NotificationDTO MapToNotificationDTO(this Notification notification)
        {
            if (notification != null)
            {
                return new NotificationDTO
                {
                    Id = notification.Id,
                    Notifications = notification.Notifications,
                    FollowerId = notification.FollowerId,
                    FollowedUserId = notification.FollowedUserId,
                    PinnedVideosId = notification.PinnedVideosId,
                    PostId = notification.PostId,
                    RatingId = notification.RatingId
                };
            }

            return null;
        }
    }
}

