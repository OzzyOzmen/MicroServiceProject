using System;
using System.Collections.Generic;

namespace WebPage.ORM.Data
{
    public partial class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
