﻿using System;
using System.Collections.Generic;

namespace PostApi.ORM.Data
{
    public partial class PinnedPost
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
    }
}
