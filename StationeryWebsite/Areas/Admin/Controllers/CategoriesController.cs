using StationeryWebsite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StationeryWebsite.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private QLBanDoDungHocTapEntities1 db = new QLBanDoDungHocTapEntities1();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
      [Bind(Include = "category_id,name,image,note")] Category category,
      HttpPostedFileBase imageFile
  )
        {
            if (ModelState.IsValid)
            {
                // Nếu có upload ảnh
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Lấy tên file gốc
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);

                    // Tạo tên mới tránh trùng
                    string newFileName = Guid.NewGuid().ToString() +
                                         System.IO.Path.GetExtension(fileName);

                    // Đường dẫn lưu
                    string path = Server.MapPath("~/Content/images/" + newFileName);

                    // Lưu file
                    imageFile.SaveAs(path);

                    // Lưu đường dẫn vào DB
                    category.image = "/Content/images/" + newFileName;
                }

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Include = "category_id,name,image,note")] Category model,
    HttpPostedFileBase imageFile
)
        {
            if (ModelState.IsValid)
            {
                var category = db.Categories.Find(model.category_id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                category.name = model.name;
                category.note = model.note;

                // 👉 Nếu có upload ảnh mới
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);

                    string folder = Server.MapPath("~/Content/images");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string fullPath = Path.Combine(folder, fileName);

                    imageFile.SaveAs(fullPath);

                    // 👉 Cập nhật ảnh mới
                    category.image = "/Content/images/" + fileName;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
