using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPage.DTO;
using WebPage.ORM.Data;
using WebPage.REPOSITORY;

namespace WebPage.SERVICES
{
    public class NotificationServices
    {
        NotificationRepository notificationRepository;
        static object _lockobject = new object();

        public NotificationServices()
        {
            lock (_lockobject)
            {
                if (notificationRepository == null)
                {
                    notificationRepository = new NotificationRepository();
                }
            }
        }
        public IEnumerable<NotificationDTO> GetAll()
        {
            return notificationRepository.GetAll().Select(x => new NotificationDTO
            {
                Id = x.Id,
                Notifications = x.Notifications,
                FollowedUserId = x.FollowedUserId,
                FollowerId = x.FollowerId,
                PinnedVideosId = x.PinnedVideosId,
                RatingId = x.RatingId,
                PostId = x.PostId

            }).ToList();
        }

        public void Add(NotificationDTO entity)
        {
            Notification notification = new Notification
            {
                Id = entity.Id,
                Notifications = entity.Notifications,
                FollowedUserId = entity.FollowedUserId,
                FollowerId = entity.FollowerId,
                PinnedVideosId = entity.PinnedVideosId,
                RatingId = entity.RatingId,
                PostId = entity.PostId
            };
            notificationRepository.Add(notification);
        }

        public void Put(NotificationDTO entity)
        {
            var notification = notificationRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (notification != null)
            {

                notification.Id = entity.Id;
                notification.Notifications = entity.Notifications;
                notification.FollowedUserId = entity.FollowedUserId;
                notification.FollowerId = entity.FollowerId;
                notification.PinnedVideosId = entity.PinnedVideosId;
                notification.RatingId = entity.RatingId;
                notification.PostId = entity.PostId;

                notificationRepository.Update(notification);

            }


        }

        public void Delete(NotificationDTO entity)
        {
            var notification = notificationRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

            if (notification != null)
            {
                notification.Id = entity.Id;
                notification.Notifications = entity.Notifications;
                notification.FollowedUserId = entity.FollowedUserId;
                notification.FollowerId = entity.FollowerId;
                notification.PinnedVideosId = entity.PinnedVideosId;
                notification.RatingId = entity.RatingId;
                notification.PostId = entity.PostId;

                notificationRepository.Delete(notification);
            }

        }

        public bool DeleteById(int Id)
        {
            return notificationRepository.DeletebyEntity(x => x.Id == Id);
        }



    }
}