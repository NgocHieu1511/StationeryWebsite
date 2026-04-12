using StationeryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StationeryWebsite.Areas.Admin.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: Admin/LoginAdmin
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Login(string user, string password)
        {
            QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

            int count = db.Users.Count(m => m.username.ToLower() == user.ToLower() && m.password == password);

            if (count == 1)
            {
                // Lưu session
                Session["user"] = user;



                // Chuyển sang trang dashboard
                return RedirectToAction("Index", "HomeAdmin");
            }
            else
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }
        }
        public ActionResult Logout()
        {
            //xóa session
            Session.Remove("user");
            //xóa session form authentication
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "LoginAdmin");
        }
    }
}
