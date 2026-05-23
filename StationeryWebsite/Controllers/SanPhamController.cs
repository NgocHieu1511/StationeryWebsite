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
            ViewBag.Reviews = db.Feedbacks
                       .Where(x => x.product_id == id)
                       .OrderByDescending(x => x.date)
                       .ToList();

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
        [HttpPost]
        public ActionResult AddReview(int productId, int rating, string message)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            // kiểm tra đã review chưa
            var feedback = db.Feedbacks
                             .FirstOrDefault(f =>
                                 f.user_id == userId &&
                                 f.product_id == productId);

            if (feedback != null)
            {
                // UPDATE
                feedback.comment = message;
                feedback.vote = rating;
                feedback.date = DateTime.Now;
            }
            else
            {
                // INSERT
                Feedback fb = new Feedback()
                {
                    user_id = userId,
                    product_id = productId,
                    comment = message,
                    vote = rating,
                    date = DateTime.Now
                };

                db.Feedbacks.Add(fb);
            }

            db.SaveChanges();

            return RedirectToAction("Details", new { id = productId });
        }
    }
}