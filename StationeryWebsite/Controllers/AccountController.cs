    using StationeryWebsite.Models;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.IO;

    namespace StationeryWebsite.Controllers
    {
        public class AccountController : Controller
        {
            QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

            // GET: Account
            public ActionResult Index()
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Login");

                int userId = (int)Session["UserId"];

                var user = db.Users.Find(userId);

                if (user == null)
                    return RedirectToAction("Login", "Login");

                var address = db.Addresses
                    .Where(a => a.user_id == userId)
                    .OrderByDescending(a => a.address_id)
                    .FirstOrDefault();

                ViewBag.UserAddress = address;

                ViewBag.TotalOrders =
                    db.Orders.Count(o => o.user_id == userId);

                ViewBag.PendingOrders =
                    db.Orders.Count(o => o.user_id == userId && o.status_id == 1);

                ViewBag.CompletedOrders =
                    db.Orders.Count(o => o.user_id == userId && o.status_id == 4);

                return View(user);
            }

            // GET: Account/Edit
            public ActionResult Edit()
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Login");

                int userId = (int)Session["UserId"];

                var user = db.Users.Find(userId);

                if (user == null)
                    return RedirectToAction("Login", "Login");

                var address = db.Addresses
                    .Where(a => a.user_id == userId)
                    .OrderByDescending(a => a.address_id)
                    .FirstOrDefault();

                ViewBag.UserAddress = address;

                return View(user);
            }

            // POST: Account/Edit
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(FormCollection form, HttpPostedFileBase avatarFile)
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Login");

                int userId = (int)Session["UserId"];

                var user = db.Users.Find(userId);

                if (user == null)
                    return RedirectToAction("Login", "Login");

                // ===== CẬP NHẬT HỌ TÊN =====
                user.first_name = form["first_name"];
                user.last_name = form["last_name"];

                // ===== UPLOAD AVATAR =====
                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    // Kiểm tra đuôi file
                    string extension =
                        Path.GetExtension(avatarFile.FileName).ToLower();

                    string[] allowedExtensions =
                    {
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".webp"
                    };

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] =
                            "Chỉ cho phép file JPG, PNG, WEBP!";

                        return RedirectToAction("Index");
                    }

                    // Giới hạn 2MB
                    if (avatarFile.ContentLength > 2 * 1024 * 1024)
                    {
                        TempData["Error"] =
                            "Ảnh tối đa 2MB!";

                        return RedirectToAction("Index");
                    }

                    // Tạo tên file mới
                    string fileName =
                        Guid.NewGuid().ToString() + extension;

                    // Folder lưu ảnh
                    string folderPath =
                        Server.MapPath("~/Content/images/avatar/");

                    // Tạo folder nếu chưa có
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Đường dẫn đầy đủ
                    string fullPath =
                        Path.Combine(folderPath, fileName);

                    // Lưu file
                    avatarFile.SaveAs(fullPath);

                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(user.image))
                    {
                        string oldImagePath =
                            Server.MapPath(user.image);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lưu DB
                    user.image =
                        "/Content/images/avatar/" + fileName;
                }

                // ===== ĐỔI MẬT KHẨU =====
                string oldPassword = form["old_password"];
                string newPassword = form["new_password"];
                string confirmPassword = form["confirm_password"];

                // Chỉ xử lý nếu có nhập password mới
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    // Bắt nhập mật khẩu cũ
                    if (string.IsNullOrWhiteSpace(oldPassword))
                    {
                        TempData["Error"] =
                            "Vui lòng nhập mật khẩu hiện tại!";

                        return RedirectToAction("Index");
                    }

                    // Kiểm tra xác nhận
                    if (newPassword != confirmPassword)
                    {
                        TempData["Error"] =
                            "Mật khẩu xác nhận không khớp!";

                        return RedirectToAction("Index");
                    }

                    // Hash mật khẩu cũ
                    string hashedOld =
                        FormsAuthentication
                        .HashPasswordForStoringInConfigFile(
                            oldPassword,
                            "SHA1"
                        );

                // So sánh DB
                if (user.password.ToLower() != hashedOld.ToLower())
                {
                        TempData["Error"] =
                            "Mật khẩu cũ không đúng!";

                        return RedirectToAction("Index");
                    }

                // Hash mật khẩu mới
                user.password = FormsAuthentication
.HashPasswordForStoringInConfigFile(newPassword, "SHA1")
.ToLower();
            }

                // ===== CẬP NHẬT ĐỊA CHỈ =====
                string specificAddress = form["specific_address"];

                if (!string.IsNullOrWhiteSpace(specificAddress))
                {
                    var existingAddress = db.Addresses
                        .Where(a => a.user_id == userId)
                        .OrderByDescending(a => a.address_id)
                        .FirstOrDefault();

                    if (existingAddress != null)
                    {
                        existingAddress.specific_address = specificAddress;
                        existingAddress.ward_name = form["ward_name"] ?? "";
                        existingAddress.district_name = form["district_name"] ?? "";
                        existingAddress.province_name = form["province_name"] ?? "";
                    }
                    else
                    {
                        db.Addresses.Add(new Address
                        {
                            user_id = userId,
                            specific_address = specificAddress,
                            ward_name = form["ward_name"] ?? "",
                            district_name = form["district_name"] ?? "",
                            province_name = form["province_name"] ?? ""
                        });
                    }
                }


            // ===== SAVE =====
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

                // Update session
                Session["FullName"] =
                    user.last_name + " " + user.first_name;

                TempData["Success"] =
                    "Cập nhật thông tin thành công!";

                return RedirectToAction("Index");
            }

            // GET: Account/Orders
            public ActionResult Orders()
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Login");

                int userId = (int)Session["UserId"];

                var orders = db.Orders
                    .Include("Order_status")
                    .Include("Order_detail.Product")
                    .Where(o => o.user_id == userId)
                    .OrderByDescending(o => o.date)
                    .ToList();

                return View(orders);
            }

            // GET: Account/OrderDetail/5
            public ActionResult OrderDetail(int id)
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Login");

                int userId = (int)Session["UserId"];

                var order = db.Orders
                    .Include("Order_status")
                    .Include("Order_detail.Product")
                    .FirstOrDefault(o =>
                        o.order_id == id &&
                        o.user_id == userId);

                if (order == null)
                {
                    TempData["Error"] =
                        "Không tìm thấy đơn hàng!";

                    return RedirectToAction("Orders");
                }

                ViewBag.ShippingAddress = db.Addresses
                    .Where(a => a.user_id == userId)
                    .OrderByDescending(a => a.address_id)
                    .FirstOrDefault();

                return View(order);
            }

            // POST: Account/CancelOrder/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult CancelOrder(int id)
            {
                if (Session["UserId"] == null)
                    return RedirectToAction("Login", "Login");

                int userId = (int)Session["UserId"];

                var order = db.Orders
                    .Include("Order_detail")
                    .FirstOrDefault(o =>
                        o.order_id == id &&
                        o.user_id == userId);

                if (order == null)
                {
                    TempData["Error"] =
                        "Không tìm thấy đơn hàng!";

                    return RedirectToAction("Orders");
                }

                if (order.status_id != 1)
                {
                    TempData["Error"] =
                        "Chỉ có thể hủy đơn đang chờ xác nhận!";

                    return RedirectToAction("OrderDetail", new { id });
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Hoàn lại tồn kho
                        foreach (var detail in order.Order_detail)
                        {
                            var product =
                                db.Products.Find(detail.product_id);

                            if (product != null)
                            {
                                product.quantity += detail.quantity;

                                product.sold_quantity =
                                    Math.Max(
                                        0,
                                        (product.sold_quantity ?? 0) - detail.quantity
                                    );
                            }
                        }

                        // Đã hủy
                        order.status_id = 5;

                        db.SaveChanges();

                        transaction.Commit();

                        TempData["Success"] =
                            "Đã hủy đơn hàng thành công!";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        TempData["Error"] =
                            "Lỗi: " + ex.Message;
                    }
                }

                return RedirectToAction("OrderDetail", new { id });
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    db.Dispose();

                base.Dispose(disposing);
            }
        }
    }