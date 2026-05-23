using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using StationeryWebsite.Models;

namespace StationeryWebsite.Controllers
{
    public class ThanhToanController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        // HIEN THI TRANG THANH TOAN
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserId"];

            var cart = db.Carts
                .Include(c => c.Product)
                .Where(c => c.user_id == userId)
                .ToList();

            if (!cart.Any())
                return RedirectToAction("Index", "GioHang");

            return View(cart);
        }

        // DAT HANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserId"];

            var cart = db.Carts
                .Include(c => c.Product)
                .Where(c => c.user_id == userId)
                .ToList();

            if (!cart.Any())
                return RedirectToAction("Index", "GioHang");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // ===== CHECK TỒN KHO =====
                    foreach (var item in cart)
                    {
                        if (item.Product == null)
                            throw new Exception("Sản phẩm không tồn tại.");

                        if (item.quantity <= 0)
                            throw new Exception("Số lượng không hợp lệ.");

                        if (item.Product.quantity < item.quantity)
                        {
                            TempData["Error"] =
                                $"Sản phẩm '{item.Product.name}' không đủ số lượng (còn {item.Product.quantity})";
                            return RedirectToAction("Index");
                        }
                    }

                    // ===== TÍNH TỔNG TIỀN =====
                    decimal total = cart.Sum(c =>
                        c.quantity * (c.Product?.price ?? 0));

                    // ===== TẠO ORDER =====
                    Order order = new Order
                    {
                        date = DateTime.Now,
                        total_price = total,
                        user_id = userId,
                        status_id = 1 // Pending
                    };

                    db.Orders.Add(order);
                    db.SaveChanges(); // lấy order_id

                    // ===== TẠO ORDER DETAIL + UPDATE STOCK =====
                    foreach (var item in cart)
                    {
                        if (item.Product == null)
                            continue;

                        Order_detail detail = new Order_detail
                        {
                            order_id = order.order_id,
                            product_id = item.product_id,
                            quantity = item.quantity,
                            item_price = item.Product.price
                        };

                        db.Order_detail.Add(detail);

                        // update tồn kho
                        item.Product.quantity -= item.quantity;
                        item.Product.sold_quantity =
                            (item.Product.sold_quantity ?? 0) + item.quantity;
                    }

                    // ===== XÓA CART =====
                    db.Carts.RemoveRange(cart);

                    db.SaveChanges();
                    transaction.Commit();

                    TempData["OrderId"] = order.order_id;
                    TempData["Success"] = "Đặt hàng thành công!";

                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }
        }

        // THANH CONG
        public ActionResult Success()
        {
            if (TempData["OrderId"] == null)
                return RedirectToAction("Index", "Home");

            int orderId = (int)TempData["OrderId"];

            var order = db.Orders
                .Include(o => o.Order_detail.Select(d => d.Product))
                .FirstOrDefault(o => o.order_id == orderId);

            if (order == null)
                return RedirectToAction("Index", "Home");

            return View(order);
        }
    }
}