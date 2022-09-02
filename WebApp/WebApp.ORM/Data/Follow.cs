using System;
using System.Collections.Generic;

namespace WebPage.ORM.Data
{
    public partial class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
    }
}
