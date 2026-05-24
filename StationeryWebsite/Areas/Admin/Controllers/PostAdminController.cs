using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using StationeryWebsite.Models;

namespace StationeryWebsite.Areas.Admin.Controllers
{
    public class PostAdminController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        // Kiểm tra quyền Admin
        private bool IsAdmin()
        {
            return Session["UserId"] != null &&
                   Session["Role"] != null &&
                   (bool)Session["Role"] == true;
        }

        private ActionResult RedirectToLogin()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login", new { area = "" });
            return RedirectToAction("Index", "Home");
        }

        // GET: Admin/PostAdmin
        public ActionResult Index(string search, string status)
        {
            if (!IsAdmin()) return RedirectToLogin();

            db.Configuration.ProxyCreationEnabled = false;

            var posts = db.Posts.AsQueryable();

            // Tìm kiếm theo tiêu đề
            if (!string.IsNullOrEmpty(search))
            {
                posts = posts.Where(p => p.title.Contains(search));
                ViewBag.Search = search;
            }

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                    posts = posts.Where(p => p.is_active == true);
                else if (status == "inactive")
                    posts = posts.Where(p => p.is_active == false || p.is_active == null);
                ViewBag.Status = status;
            }

            var postList = posts.OrderByDescending(p => p.created_at).ToList();

            return View(postList);
        }

        // GET: Admin/PostAdmin/Create
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToLogin();
            return View();
        }

        // POST: Admin/PostAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Cho phép HTML trong content
        public ActionResult Create(Post post)
        {
            if (!IsAdmin()) return RedirectToLogin();

            try
            {
                post.created_at = DateTime.Now;
                post.updated_at = DateTime.Now;
                post.user_id = (int)Session["UserId"];

                // Nếu không chọn is_active, mặc định là true
                if (post.is_active == null)
                    post.is_active = true;

                // Tạo slug từ tiêu đề (nếu chưa có)
                if (string.IsNullOrEmpty(post.slug))
                {
                    post.slug = GenerateSlug(post.title);
                }

                db.Posts.Add(post);
                db.SaveChanges();

                TempData["Success"] = "Thêm bài viết thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return View(post);
            }
        }

        // GET: Admin/PostAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var post = db.Posts.Find(id);
            if (post == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết!";
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // POST: Admin/PostAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Post post)
        {
            if (!IsAdmin()) return RedirectToLogin();

            try
            {
                var existingPost = db.Posts.Find(post.post_id);
                if (existingPost != null)
                {
                    existingPost.title = post.title;
                    existingPost.content = post.content;
                    existingPost.summary = post.summary;
                    existingPost.image = post.image;
                    existingPost.is_active = post.is_active;
                    existingPost.updated_at = DateTime.Now;

                    // Cập nhật slug nếu tiêu đề thay đổi
                    if (existingPost.title != post.title || string.IsNullOrEmpty(existingPost.slug))
                    {
                        existingPost.slug = GenerateSlug(post.title);
                    }

                    db.SaveChanges();

                    TempData["Success"] = "Cập nhật bài viết thành công!";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Không tìm thấy bài viết!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return View(post);
        }

        // POST: Admin/PostAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Không có quyền!" });

            try
            {
                var post = db.Posts.Find(id);
                if (post != null)
                {
                    db.Posts.Remove(post);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa thành công!" });
                }

                return Json(new { success = false, message = "Không tìm thấy bài viết!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: Admin/PostAdmin/ToggleStatus/5
        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false });

            var post = db.Posts.Find(id);
            if (post != null)
            {
                post.is_active = !(post.is_active ?? true);
                post.updated_at = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, is_active = post.is_active });
            }

            return Json(new { success = false });
        }

        // Hàm tạo slug từ tiêu đề
        private string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title)) return "";

            // Chuyển sang không dấu
            string slug = RemoveAccents(title);

            // Thay thế khoảng trắng và ký tự đặc biệt
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-zA-Z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            slug = slug.Trim('-').ToLower();

            // Thêm timestamp để đảm bảo unique
            slug = slug + "-" + DateTime.Now.Ticks.ToString().Substring(8);

            return slug;
        }

        private string RemoveAccents(string text)
        {
            string[] signs = new string[] {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };

            for (int i = 1; i < signs.Length; i++)
            {
                for (int j = 0; j < signs[i].Length; j++)
                {
                    text = text.Replace(signs[i][j], signs[0][i - 1]);
                }
            }

            return text;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}