using System;
using System.Linq;
using System.Web.Mvc;
using StationeryWebsite.Models; // nhớ import model

namespace StationeryWebsite.Controllers
{
    public class LoginController : Controller
    {
        private QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1(); // thay bằng DbContext của bạn

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.username == username && u.password == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }

            Session["UserId"] = user.user_id;
            Session["Username"] = user.username;
            Session["Role"] = user.role;

            if (user.role == true)
            {
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }
    }
}