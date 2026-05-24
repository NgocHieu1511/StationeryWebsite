using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using StationeryWebsite.Models;

namespace StationeryWebsite.Controllers
{
    public class PostController : Controller
    {
        QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        // GET: Post - Danh sách bài viết
        public ActionResult Index(int page = 1)
        {
            int pageSize = 6; // Số bài viết mỗi trang

            // Chỉ lấy bài đang hiển thị (is_active = true)
            var posts = db.Posts
                .Where(p => p.is_active == true)
                .OrderByDescending(p => p.created_at);

            // Tổng số bài viết
            int totalPosts = posts.Count();
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            // Lấy bài viết cho trang hiện tại
            var postList = posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền dữ liệu phân trang qua ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalPosts = totalPosts;

            // Lấy bài viết mới nhất cho sidebar
            ViewBag.RecentPosts = db.Posts
                .Where(p => p.is_active == true)
                .OrderByDescending(p => p.created_at)
                .Take(5)
                .ToList();

            return View(postList);
        }

        // GET: Post/Detail/5 hoặc Post/Detail?slug=ten-bai-viet
        public ActionResult Detail(int? id, string slug)
        {
            Post post = null;

            // Tìm theo id
            if (id.HasValue && id > 0)
            {
                post = db.Posts.FirstOrDefault(p => p.post_id == id && p.is_active == true);
            }

            // Nếu không tìm thấy theo id, tìm theo slug
            if (post == null && !string.IsNullOrEmpty(slug))
            {
                post = db.Posts.FirstOrDefault(p => p.slug == slug && p.is_active == true);
            }

            if (post == null)
            {
                TempData["Error"] = "Bài viết không tồn tại hoặc đã bị ẩn!";
                return RedirectToAction("Index");
            }

            // Tăng lượt xem
            
            db.SaveChanges();

            // Lấy bài viết mới nhất cho sidebar (trừ bài hiện tại)
            ViewBag.RecentPosts = db.Posts
                .Where(p => p.is_active == true && p.post_id != post.post_id)
                .OrderByDescending(p => p.created_at)
                .Take(5)
                .ToList();

            return View(post);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}