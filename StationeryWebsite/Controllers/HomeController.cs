using StationeryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryWebsite.Controllers
{
    public class HomeController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CategorySection()
        {
            var categories = db.Categories.OrderBy(x => x.category_id).ToList();

            return PartialView(categories);
        }
        public ActionResult FeaturedProducts()
        {
            var products = db.Products
                             .Where(p => p.is_featured == true && p.status == true)
                             .OrderByDescending(p => p.created_at)
                             .Take(8)
                             .ToList();

            return PartialView(products);
        }
        public PartialViewResult SaleProducts()
        {
            var saleProducts = db.Products
                                 .Where(p => p.is_sale == true && p.status == true)
                                 .ToList();

            return PartialView(saleProducts);
        }
    }
}