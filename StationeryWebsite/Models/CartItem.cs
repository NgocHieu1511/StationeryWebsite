using System;

namespace StationeryWebsite.Models
{
    public class CartItem
    {
        public int product_id { get; set; }

        public string name { get; set; }

        public string image { get; set; }

        public decimal price { get; set; }

        public int quantity { get; set; }

        public decimal ThanhTien
        {
            get { return price * quantity; }
        }
    }
}