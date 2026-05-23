using StationeryWebsite.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace StationeryWebsite.Controllers
{
    public class YeuThichController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        // GET: YeuThich
        public ActionResult YeuThich()
        {
            List<WishlistItem> wishlist = Session["Wishlist"] as List<WishlistItem>;

            if (wishlist == null)
            {
                wishlist = new List<WishlistItem>();
            }

            return View(wishlist);
        }

        // ADD TO WISHLIST
        public ActionResult AddToWishlist(int id)
        {
            List<WishlistItem> wishlist = Session["Wishlist"] as List<WishlistItem>;

            if (wishlist == null)
            {
                wishlist = new List<WishlistItem>();
            }

            var product = db.Products.Find(id);

            if (product != null)
            {
                bool exists = wishlist.Any(x => x.ProductID == id);

                if (!exists)
                {
                    wishlist.Add(new WishlistItem
                    {
                        ProductID = product.product_id,
                        ProductName = product.name,
                        Image = product.image,
                        Price = product.price 
                    });
                }
            }

            Session["Wishlist"] = wishlist;

            return RedirectToAction("YeuThich");
        }

        // REMOVE
        public ActionResult Remove(int id)
        {
            List<WishlistItem> wishlist = Session["Wishlist"] as List<WishlistItem>;

            if (wishlist != null)
            {
                var item = wishlist.FirstOrDefault(x => x.ProductID == id);

                if (item != null)
                {
                    wishlist.Remove(item);
                }
            }

            Session["Wishlist"] = wishlist;

            return RedirectToAction("YeuThich");
        }
    }
}