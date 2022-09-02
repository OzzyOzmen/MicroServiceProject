using System;
using System.Collections.Generic;

namespace WebPage.ORM.Data
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Notifications { get; set; } = null!;
        public int PinnedVideosId { get; set; }
        public int FollowerId { get; set; }
        public int FollowedUserId { get; set; }
        public int RatingId { get; set; }
    }
}
