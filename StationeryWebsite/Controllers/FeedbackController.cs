using System;
using System.Linq;
using System.Web.Mvc;
using StationeryWebsite.Models;

namespace StationeryWebsite.Controllers
{
    public class FeedbackController : Controller
    {
        QLBanDoDungHocTapEntities1 db =
            new QLBanDoDungHocTapEntities1();

        // THÊM PHẢN HỒI
        [HttpPost]
        public ActionResult AddFeedback(int product_id,
                                        string comment,
                                        int vote)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            User user = (User)Session["user"];

            Feedback fb = new Feedback();

            fb.user_id = user.user_id;
            fb.product_id = product_id;
            fb.comment = comment;
            fb.vote = vote;
            fb.date = DateTime.Now;

            db.Feedbacks.Add(fb);

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}