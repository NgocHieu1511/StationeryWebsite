using StationeryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace StationeryWebsite.Controllers
{
    public class GioHangController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();
    

        // =========================
        // HIỂN THỊ GIỎ HÀNG
        // =========================
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserId"];

            var cart = db.Carts
                .Where(c => c.user_id == userId)
                .ToList();

            return View(cart);
        }

        // =========================
        // THÊM VÀO GIỎ
        // =========================
        public ActionResult AddToCart(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserId"];

            var item = db.Carts.FirstOrDefault(c =>
                c.product_id == id &&
                c.user_id == userId);

            if (item == null)
            {
                Cart cart = new Cart()
                {
                    product_id = id,
                    user_id = userId,
                    quantity = 1
                };

                db.Carts.Add(cart);
            }
            else
            {
                item.quantity += 1;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // XÓA SẢN PHẨM
        // =========================
        public ActionResult Delete(int id)
        {
            var item = db.Carts.Find(id);

            if (item == null)
                return RedirectToAction("Index");

            db.Carts.Remove(item);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // TĂNG SỐ LƯỢNG
        // =========================
        public ActionResult IncreaseQuantity(int id)
        {
            var item = db.Carts.Find(id);

            if (item != null)
            {
                item.quantity += 1;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // =========================
        // GIẢM SỐ LƯỢNG
        // =========================
        public ActionResult DecreaseQuantity(int id)
        {
            var item = db.Carts.Find(id);

            if (item != null)
            {
                if (item.quantity > 1)
                {
                    item.quantity -= 1;
                }
                else
                {
                    db.Carts.Remove(item);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult BuyNow(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserId"];

            // kiểm tra sản phẩm đã có trong giỏ chưa
            var item = db.Carts.FirstOrDefault(c =>
                c.product_id == id &&
                c.user_id == userId);

            if (item == null)
            {
                Cart cart = new Cart()
                {
                    product_id = id,
                    user_id = userId,
                    quantity = 1
                };

                db.Carts.Add(cart);
            }
            else
            {
                item.quantity += 1;
            }

            db.SaveChanges();

            // chuyển sang thanh toán
            return RedirectToAction("Index", "ThanhToan");
        }
    }
}