using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTH.Models;

namespace WebApplicationTH.Controllers
{
    public class UserController : Controller
    {
        private QLBansachEntities db = new QLBansachEntities();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection collection, KHACHHANG model)
        {
            var hoten = collection["HoTen"];
            var tendn = collection["Taikhoan"];
            var matkhau = collection["Matkhau"];
            var matkhaunhaplai = collection["Matkhaunhaplai"];
            var diachi = collection["DiachiKH"];
            var email = collection["Email"];
            var dienthoai = collection["DienthoaiKH"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);


            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            }
            else if (matkhau != matkhaunhaplai)
            {
                ViewData["Loi4"] = "Mật khẩu không trùng";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = "Phải nhập lại địa chỉ";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập lại điện thoại";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "Phải nhập lại ngày sinh";
            }
            else
            {
                model.HoTen = hoten;
                model.Taikhoan = tendn;
                model.Matkhau = matkhau;
                model.Email = email;
                model.DiachiKH = diachi;
                model.DienthoaiKH = dienthoai;
                model.Ngaysinh = DateTime.Parse(ngaysinh);

                db.KHACHHANGs.Add(model);
                db.SaveChanges();

                return RedirectToAction("LogIn");
            }

            return View(model);
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
            } else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);
                if (kh != null)
                {
                    ViewBag.ThongBaoThanhCong = "Đăng nhập thành công!!";
                    Session["Taikhoan"] = kh;
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    ViewBag.ThongBaoThatBai = "Tên đăng nhập hoặc mật khấu không đúng!!";
                }
            }

            return View();
        }

        public ActionResult LogOut()
        {
            Session["Taikhoan"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}