using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryWebsite.Models; // nhớ import model

namespace StationeryWebsite.Controllers
{
    public class AboutController : Controller
    {
        private QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1(); // thay bằng DbContext của bạn
        // GET: About
        public ActionResult Index()
        {
            // lấy review mới nhất
            var reviews = db.Feedbacks
                            .Include("User")
                            .Include("Product")
                            .OrderByDescending(x => x.date)
                            .Take(6)
                            .ToList();

            ViewBag.Reviews = reviews;
            return View();
        }
    }
}