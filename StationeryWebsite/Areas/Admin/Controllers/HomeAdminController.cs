using StationeryWebsite.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace StationeryWebsite.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        public ActionResult Index()
        {
            // Thống kê
            ViewBag.TotalProducts = db.Products.Count();
            ViewBag.TotalUsers = db.Users.Count();
            ViewBag.TotalOrders = db.Orders.Count();

            // Tổng doanh thu
            decimal totalRevenue = db.Orders
                .Where(x => x.status_id == 1)
                .Sum(x => (decimal?)x.total_price) ?? 0;

            ViewBag.TotalRevenue = totalRevenue;

            // Đơn hàng mới
            var latestOrders = db.Orders
                .OrderByDescending(x => x.order_id)
                .Take(5)
                .ToList();

            // Sản phẩm sắp hết
            var lowStockProducts = db.Products
                .Where(x => x.quantity < 10)
                .OrderBy(x => x.quantity)
                .Take(5)
                .ToList();

            ViewBag.LowStockProducts = lowStockProducts;

            return View(latestOrders);
        }
    }
}