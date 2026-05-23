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

            // Lấy thông tin user để hiển thị sẵn lên form
            var user = db.Users.Find(userId);
            ViewBag.UserInfo = user;

            // Lấy địa chỉ gần nhất của user (nếu có)
            var latestAddress = db.Addresses
                .Where(a => a.user_id == userId)
                .OrderByDescending(a => a.address_id)
                .FirstOrDefault();
            ViewBag.LatestAddress = latestAddress;

            return View(cart);
        }

        // DAT HANG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(FormCollection form)
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
                    // ===== 1. LƯU THÔNG TIN GIAO HÀNG VÀO BẢNG ADDRESS =====
                    string specificAddress = form["address"];
                    string ward = form["ward"];
                    string district = form["district"];
                    string province = form["province"];

                    // Chỉ lưu địa chỉ nếu có nhập đầy đủ
                    if (!string.IsNullOrEmpty(specificAddress) && !string.IsNullOrEmpty(province))
                    {
                        Address newAddress = new Address
                        {
                            user_id = userId,
                            specific_address = specificAddress,
                            ward_name = ward ?? "",
                            district_name = district ?? "",
                            province_name = province
                        };

                        db.Addresses.Add(newAddress);
                    }

                    // ===== 2. LẤY THÔNG TIN PHÍ SHIP =====
                    decimal shippingFee = 15000; // Mặc định
                    if (!string.IsNullOrEmpty(form["shipping_fee"]))
                    {
                        decimal.TryParse(form["shipping_fee"], out shippingFee);
                    }

                    // ===== 3. LẤY PHƯƠNG THỨC THANH TOÁN =====
                    string paymentMethod = form["payment_method"] ?? "cod";

                    // ===== 4. CHECK TỒN KHO =====
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

                    // ===== 5. TÍNH TỔNG TIỀN (Đã bao gồm phí ship) =====
                    decimal subtotal = cart.Sum(c => c.quantity * (c.Product?.price ?? 0));
                    decimal total = subtotal + shippingFee;

                    // ===== 6. TẠO ORDER =====
                    Order order = new Order
                    {
                        date = DateTime.Now,
                        total_price = total,
                        user_id = userId,
                        status_id = 1 // Chờ xác nhận
                    };

                    db.Orders.Add(order);
                    db.SaveChanges(); // Lấy order_id

                    // ===== 7. TẠO ORDER DETAIL + UPDATE STOCK =====
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

                        // Cập nhật tồn kho
                        item.Product.quantity -= item.quantity;
                        item.Product.sold_quantity =
                            (item.Product.sold_quantity ?? 0) + item.quantity;
                    }

                    // ===== 8. XÓA CART =====
                    db.Carts.RemoveRange(cart);

                    db.SaveChanges();
                    transaction.Commit();

                    // ===== 9. LƯU THÔNG BÁO + CHUYỂN HƯỚNG =====
                    TempData["OrderId"] = order.order_id;
                    TempData["ShippingFee"] = shippingFee;
                    TempData["PaymentMethod"] = paymentMethod;
                    TempData["Success"] = "Đặt hàng thành công!";

                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    // Lấy lỗi chi tiết từ Inner Exception
                    string errorMessage = ex.Message;
                    Exception innerEx = ex.InnerException;

                    while (innerEx != null)
                    {
                        errorMessage += " | Inner: " + innerEx.Message;
                        innerEx = innerEx.InnerException;
                    }

                    TempData["Error"] = "Có lỗi xảy ra: " + errorMessage;
                    return RedirectToAction("Index");
                }
            }
        }

        // THANH CONG
        // THANH CONG
        public ActionResult Success()
        {
            if (TempData["OrderId"] == null)
                return RedirectToAction("Index", "Home");

            int orderId = (int)TempData["OrderId"];

            // SỬA: Dùng Include dạng string cho EF6
            var order = db.Orders
                .Include("Order_status")
                .Include("Order_detail")
                .Include("Order_detail.Product")
                .FirstOrDefault(o => o.order_id == orderId);

            if (order == null)
                return RedirectToAction("Index", "Home");

            // Lấy địa chỉ gần nhất của user
            var shippingAddress = db.Addresses
                .Where(a => a.user_id == order.user_id)
                .OrderByDescending(a => a.address_id)
                .FirstOrDefault();

            ViewBag.ShippingAddress = shippingAddress;
            ViewBag.ShippingFee = TempData["ShippingFee"] ?? 15000;
            ViewBag.PaymentMethod = TempData["PaymentMethod"] ?? "cod";

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}