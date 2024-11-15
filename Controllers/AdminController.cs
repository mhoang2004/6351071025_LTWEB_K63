using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTH.Models;
using PagedList.Mvc;
using PagedList;
using System.IO;

namespace WebApplicationTH.Controllers
{
    public class BookChartViewModel
    {
        public string ChuDe { get; set; }
        public int SoLuong { get; set; }
    }
    public class AdminController : Controller
    {

        private QLBansachEntities db = new QLBansachEntities();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(FormCollection collection)
        {
            var tendn = collection["Taikhoan"];
            var matkhau = collection["Matkhau"];

            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tài khoản";
            }
            else if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin kh = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (kh != null)
                {
                    ViewBag.ThongBaoThanhCong = "Đăng nhập thành công!!";
                    Session["Admin"] = kh;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ThongBaoThatBai = "Tên đăng nhập hoặc mật khấu không đúng!!";
                }
            }

            return View();
        }

        public ActionResult Books(int? page)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            int pageNumber = page ?? 1;
            int pageSize = 7;
            var books = db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize);
            return View(books);
        }

        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            var book = db.SACHes.Find(id);
            return View(book);
        }

        
        public ActionResult Details(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            var book = db.SACHes.Find(id);
            return View(book);
        }

        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            var book = db.SACHes.Find(id);
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SACH sach)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if (ModelState.IsValid)
            {
                var existingSach = db.SACHes.SingleOrDefault(n => n.Masach == sach.Masach);
                existingSach.Tensach = sach.Tensach;
                existingSach.Giaban = sach.Giaban;
                existingSach.Mota = sach.Mota;
                existingSach.Anhbia = sach.Anhbia;
                existingSach.Ngaycapnhat = sach.Ngaycapnhat;
                existingSach.Soluongton = sach.Soluongton;
                existingSach.MaCD = sach.MaCD;
                existingSach.MaNXB = sach.MaNXB;

                db.SaveChanges();

                return RedirectToAction("Books");
            }

            // Return the view with validation errors
            return View(sach);
        }

        public ActionResult Create()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        public ActionResult Create(SACH sach, HttpPostedFileBase Anhbia)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if (Anhbia == null) {
                ViewData["Loi"] = "Vui lòng nhập ảnh bìa!";
                return View();
            }

            var folderPath = Server.MapPath("~/Content/books/");
            var fileName = Path.GetFileName(Anhbia.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            Anhbia.SaveAs(filePath);

            sach.Anhbia = fileName;

            db.SACHes.Add(sach);
            db.SaveChanges();

            return RedirectToAction("Books");
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("LogIn");
            }

            var book = db.SACHes.Find(id);
            if (book != null)
            {
                db.SACHes.Remove(book);
                db.SaveChanges();
            }

            return RedirectToAction("Books"); // Redirect to the list of books
        }

        public ActionResult SanphamChartData()
        {
            var data = db.SACHes
                .Where(s => s.CHUDE != null)
                .GroupBy(s => s.CHUDE.TenChuDe)
                .Select(g => new BookChartViewModel
                {
                    ChuDe = g.Key,
                    SoLuong = g.Count()
                }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Chart()
        {
            return View();
        }
    }
}