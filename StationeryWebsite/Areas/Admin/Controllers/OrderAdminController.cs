using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using StationeryWebsite.Models;

namespace StationeryWebsite.Areas.Admin.Controllers
{
    public class OrderAdminController : Controller
    {
        private QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        // Hàm kiểm tra quyền Admin
        private bool IsAdmin()
        {
            return Session["UserId"] != null &&
                   Session["Role"] != null &&
                   (bool)Session["Role"] == true;
        }

        // Hàm chuyển hướng khi không có quyền
        private ActionResult RedirectToLogin()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login"); // Về trang login chung
            else
                return RedirectToAction("Index", "Home"); // User thường về trang chủ
        }

        public ActionResult Index(string searchOrderId, int? statusFilter, DateTime? fromDate, DateTime? toDate)
        {
            // Kiểm tra quyền Admin
            if (!IsAdmin())
                return RedirectToLogin();

            // Lấy tất cả đơn hàng
            var orders = db.Orders
                .Include("Order_status")
                .Include("User")
                .Include("Order_detail.Product")
                .AsQueryable();

            // Lọc theo mã đơn hàng
            if (!string.IsNullOrEmpty(searchOrderId))
            {
                if (int.TryParse(searchOrderId, out int orderId))
                {
                    orders = orders.Where(o => o.order_id == orderId);
                }
            }

            // Lọc theo trạng thái
            if (statusFilter.HasValue && statusFilter.Value > 0)
            {
                orders = orders.Where(o => o.status_id == statusFilter.Value);
            }

            // Lọc theo ngày
            if (fromDate.HasValue)
            {
                orders = orders.Where(o => o.date >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                orders = orders.Where(o => o.date <= toDate.Value);
            }

            // Sắp xếp mới nhất trước
            orders = orders.OrderByDescending(o => o.date);

            // Lưu các giá trị filter để hiển thị lại trên form
            ViewBag.SearchOrderId = searchOrderId;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            // Lấy danh sách trạng thái cho dropdown
            ViewBag.OrderStatuses = db.Order_status.ToList();

            // Thống kê
            var orderList = orders.ToList();
            ViewBag.TotalOrders = orderList.Count;
            ViewBag.TotalRevenue = orderList.Sum(o => o.total_price);
            ViewBag.PendingOrders = orderList.Count(o => o.status_id == 1);
            ViewBag.ProcessingOrders = orderList.Count(o => o.status_id == 2 || o.status_id == 3);
            ViewBag.CompletedOrders = orderList.Count(o => o.status_id == 4);
            ViewBag.CancelledOrders = orderList.Count(o => o.status_id == 5);

            return View(orderList);
        }

        // GET: Admin/OrderAdmin/ChiTietDonHang/5
        public ActionResult ChiTietDonHang(int id)
        {
            if (!IsAdmin())
                return RedirectToLogin();

            var order = db.Orders
                .Include("Order_status")
                .Include("User")
                .Include("Order_detail.Product")
                .FirstOrDefault(o => o.order_id == id);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index");
            }

            // Lấy danh sách trạng thái để cập nhật
            ViewBag.OrderStatuses = db.Order_status.ToList();

            // Lấy địa chỉ giao hàng của user
            ViewBag.UserAddress = db.Addresses
                .Where(a => a.user_id == order.user_id)
                .OrderByDescending(a => a.address_id)
                .FirstOrDefault();

            return View(order);
        }

        // POST: Admin/OrderAdmin/CapNhatTrangThai
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatTrangThai(int orderId, int statusId, string adminNote)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Không có quyền truy cập!" });

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = db.Orders
                        .Include("Order_detail")
                        .FirstOrDefault(o => o.order_id == orderId);

                    if (order == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                    }

                    int oldStatus = order.status_id;

                    // Kiểm tra logic chuyển trạng thái hợp lệ
                    if (!IsValidStatusTransition(oldStatus, statusId))
                    {
                        return Json(new { success = false, message = "Không thể chuyển sang trạng thái này!" });
                    }

                    // Nếu hủy đơn hàng, hoàn trả số lượng sản phẩm
                    if (statusId == 5 && oldStatus != 5)
                    {
                        foreach (var detail in order.Order_detail)
                        {
                            var product = db.Products.Find(detail.product_id);
                            if (product != null)
                            {
                                product.quantity += detail.quantity;
                                product.sold_quantity = (product.sold_quantity ?? 0) - detail.quantity;
                                if (product.sold_quantity < 0) product.sold_quantity = 0;
                            }
                        }
                    }

                    // Nếu khôi phục từ trạng thái hủy, trừ lại số lượng
                    if (oldStatus == 5 && statusId != 5)
                    {
                        foreach (var detail in order.Order_detail)
                        {
                            var product = db.Products.Find(detail.product_id);
                            if (product != null)
                            {
                                if (product.quantity < detail.quantity)
                                {
                                    return Json(new { success = false, message = $"Sản phẩm {product.name} không đủ số lượng để khôi phục!" });
                                }
                                product.quantity -= detail.quantity;
                                product.sold_quantity = (product.sold_quantity ?? 0) + detail.quantity;
                            }
                        }
                    }

                    order.status_id = statusId;

                    db.SaveChanges();
                    transaction.Commit();

                    string statusName = db.Order_status.Find(statusId)?.description ?? "Không xác định";
                    return Json(new
                    {
                        success = true,
                        message = $"Đã cập nhật trạng thái thành: {statusName}",
                        newStatus = statusName,
                        statusId = statusId
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
        }

        private bool IsValidStatusTransition(int oldStatus, int newStatus)
        {
            if (oldStatus == 4 && newStatus != 4)
                return false;

            if (oldStatus == newStatus)
                return true;

            switch (oldStatus)
            {
                case 1: return newStatus == 2 || newStatus == 5;
                case 2: return newStatus == 3 || newStatus == 5;
                case 3: return newStatus == 4;
                case 4: return false;
                case 5: return newStatus == 1;
                default: return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaDonHang(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Không có quyền truy cập!" });

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = db.Orders
                        .Include("Order_detail")
                        .FirstOrDefault(o => o.order_id == id);

                    if (order == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                    }

                    if (order.status_id != 5)
                    {
                        foreach (var detail in order.Order_detail)
                        {
                            var product = db.Products.Find(detail.product_id);
                            if (product != null)
                            {
                                product.quantity += detail.quantity;
                                product.sold_quantity = (product.sold_quantity ?? 0) - detail.quantity;
                                if (product.sold_quantity < 0) product.sold_quantity = 0;
                            }
                        }
                    }

                    db.Order_detail.RemoveRange(order.Order_detail);
                    db.Orders.Remove(order);

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true, message = "Đã xóa đơn hàng thành công!" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
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