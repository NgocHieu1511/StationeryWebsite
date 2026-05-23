using StationeryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;

namespace StationeryWebsite.Controllers
{
    public class SanPhamController : Controller
    {
        private QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();
        // GET: SanPham
        public ActionResult SanPham(int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var products = db.Products
                             .Where(x => x.status == true)
                             .OrderByDescending(x => x.created_at)
                             .ToPagedList(pageNumber, pageSize);

            return View(products);
        }
        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);

            var relatedProducts = db.Products
                                    .Where(x => x.category_id == product.category_id
                                             && x.product_id != id)
                                    .Take(4)
                                    .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
        // Partial Product
        public PartialViewResult ProductPartial(int? page)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            var products = db.Products
                             .Where(p => p.status == true)
                             .OrderByDescending(p => p.created_at)
                             .ToPagedList(pageNumber, pageSize);

            return PartialView(products);
        }
        public ActionResult Category(int id, int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            var products = db.Products
                              .Where(x => x.category_id == id && x.status == true)
                              .OrderByDescending(x => x.created_at)
                              .ToPagedList(pageNumber, pageSize);

            ViewBag.CategoryName = category.name;
            ViewBag.CategoryId = id;

            return View(products);
        }
    }
}