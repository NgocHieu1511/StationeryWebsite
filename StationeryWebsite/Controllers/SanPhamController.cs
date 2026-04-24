using StationeryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace StationeryWebsite.Controllers
{
    public class SanPhamController : Controller
    {
        private QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();
        // GET: SanPham
        public ActionResult SanPham()
        {
            var products = db.Products.ToList();
            return View(products);
        }
        public ActionResult Details(int id)
        {
            var product = db.Products
                            .Include(x => x.Category)
                            .FirstOrDefault(x => x.product_id == id);

            if (product == null)
            {
                return RedirectToAction("SanPham");
            }

            return View(product);
        }
    }
}