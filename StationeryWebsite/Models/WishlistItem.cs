using System;

namespace StationeryWebsite.Models
{
    public class WishlistItem
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }
    }
}