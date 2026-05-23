using System.Linq;
using System.Web.Mvc;
using StationeryWebsite.Models;

namespace StationeryWebsite.Areas.Admin.Controllers
{
    public class FeedbackAdminController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        public ActionResult Index()
        {
            var list = db.Feedbacks
                         .OrderByDescending(x => x.date)
                         .ToList();

            return View(list);
        }

        public ActionResult Delete(int productId, int userId)
        {
            var fb = db.Feedbacks
                       .FirstOrDefault(x =>
                            x.product_id == productId &&
                            x.user_id == userId);

            if (fb != null)
            {
                db.Feedbacks.Remove(fb);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}