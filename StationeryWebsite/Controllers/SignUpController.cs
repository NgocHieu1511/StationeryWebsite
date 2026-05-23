using System;
using System.Linq;
using System.Web.Mvc;
using StationeryWebsite.Models;

namespace StationeryWebsite.Controllers
{
    public class SignUpController : Controller
    {
        QLBanDoDungHocTapEntities1 db =
            new QLBanDoDungHocTapEntities1();

        // GET: SignUp
        public ActionResult Index()
        {
            return View();
        }

        // HIỂN THỊ FORM ĐĂNG KÝ
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        // XỬ LÝ ĐĂNG KÝ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User user, string ConfirmPassword)
        {
            // KIỂM TRA MODEL
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // KIỂM TRA MẬT KHẨU
            if (user.password != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu không khớp";
                return View(user);
            }

            // KIỂM TRA EMAIL
            var checkEmail = db.Users
                .FirstOrDefault(u => u.email == user.email);

            if (checkEmail != null)
            {
                ViewBag.Error = "Email đã tồn tại";
                return View(user);
            }

            // KIỂM TRA USERNAME
            var checkUsername = db.Users
                .FirstOrDefault(u => u.username == user.username);

            if (checkUsername != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại";
                return View(user);
            }

            // GÁN ROLE
            user.role = false;

            // GÁN STATUS
            user.status = true;

            // NGÀY TẠO
            user.created_at = DateTime.Now;

            // MÃ HÓA PASSWORD (nên dùng)
            // user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

            // THÊM USER
            db.Users.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công";

            return RedirectToAction("Login", "Login");
        }
    }
}