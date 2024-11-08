using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using WebApplicationTH.Models;

namespace WebApplicationTH.Controllers
{
    public class CartController : Controller
    {
        private QLBansachEntities db = new QLBansachEntities();

        public List<Cart> Laygiohang()
        {
            List<Cart> cartList = Session["Giohang"] as List<Cart>;
            if(cartList == null)
            {
                cartList = new List<Cart>();
                Session["Giohang"] = cartList;
            }
            return cartList;
        }

        public ActionResult ThemGiohang(int iMasach, string strURL) 
        { 
            List<Cart> cartList = Laygiohang();
            Cart sp = cartList.Find(n => n.iMasach == iMasach);
            if(sp == null)
            {
                sp = new Cart(iMasach);
                cartList.Add(sp);
            } else
            {
                sp.iSoluong++;
            }
            return Redirect(strURL);
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Cart> cartList = Session["Giohang"] as List<Cart>;
            if (cartList != null)
            {
                iTongSoLuong = cartList.Sum(n => n.iSoluong);

            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<Cart> cartList = Session["Giohang"] as List<Cart>;
            if(cartList != null)
            {
                iTongTien = cartList.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        // GET: Cart
        public ActionResult Index()
        {
            List<Cart> cartList = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(cartList);
        }
        public ActionResult Edit(int id, FormCollection f)
        {
            List<Cart> cartList = Laygiohang();
            Cart cart = cartList.SingleOrDefault(n => n.iMasach == id);
            if (cart != null)
            {
                cart.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            List<Cart> cartList = Laygiohang();
            Cart cart = cartList.SingleOrDefault(n => n.iMasach == id);
            if (cart != null)
            {
                cartList.RemoveAll(n => n.iMasach == id);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Clear()
        {
            Session["Giohang"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Cart> cartList = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(cartList);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Cart> cartList = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            string ngaygiao = collection["Ngaygiao"];
            DateTime parsedNgaygiao;

            if (DateTime.TryParseExact(ngaygiao, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedNgaygiao))
            {
                ddh.Ngaygiao = parsedNgaygiao;
            }
            ddh.Tinhtranggiaohang = false;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();

            foreach(var item in cartList)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.Masach = item.iMasach;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal) item.dDongia;
                db.CHITIETDONTHANGs.Add(ctdh);
            }
            db.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang");
        }

        public ActionResult Xacnhandonhang() {
            return View();
        }
    }
}