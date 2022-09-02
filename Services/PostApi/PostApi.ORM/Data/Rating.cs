using System;
using System.Collections.Generic;

namespace PostApi.ORM.Data
{
    public partial class Rating
    {
        public int Id { get; set; }
        public int RatingCount { get; set; }
        public int PostId { get; set; }
    }
}
