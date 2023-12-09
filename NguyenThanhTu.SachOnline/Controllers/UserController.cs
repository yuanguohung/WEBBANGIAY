using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using BCrypt;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
namespace NguyenThanhTu.SachOnline.Controllers
{
    public class UserController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap( string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection, string returnUrl)
        {
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
                //ViewBag.Err2 = "Phải nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatKhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    if (collection["remember"].Contains("true"))
                    {
                        Response.Cookies["TenDN"].Value = sTenDN;
                        Response.Cookies["MatKhau"].Value = sMatKhau;
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(-1);
                    }
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        // Nếu returnUrl hợp lệ, chuyển hướng đến returnUrl
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        // Nếu không có returnUrl hoặc returnUrl không hợp lệ, chuyển hướng đến trang mặc định
                        return RedirectToAction("Index", "SachOnline");
                    }
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            // Xóa phiên đăng nhập của người dùng
            Session.Remove("TaiKhoan");

            // Chuyển hướng người dùng về trang chủ hoặc trang đăng nhập
            return RedirectToAction("Index", "SachOnline"); // TrangChu là tên hành động hoặc đường dẫn trang chủ của bạn.
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection f, KHACHHANG kh)
        {
            var sHoTen = f["HoTen"];
            var sTaiKhoan = f["TaiKhoan"];
            var sMatKhau = f["MatKhau"];
            var sMatKhauNhapLai = f["MatKhauNL"];
            var sDiaChi = f["DiaChi"];
            var sEmail = f["Email"];
            var sDienThoai = f["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", f["NgaySinh"]);

            if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err1"] = "Phải nhập mật khẩu";
            }
            if (String.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err2"] = "Phải nhập lại mật khẩu";
            }
            else if (sMatKhau != sMatKhauNhapLai)
            {
                ViewData["err4"] = "Mật khẩu nhập lại không hợp";
            }

            else if (db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewBag.ThongBao = "Email đã được sử dụng";
            }
            else if (ModelState.IsValid)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(sMatKhau);
                    byte[] hashBytes = sha256.ComputeHash(bytes);
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        builder.Append(hashBytes[i].ToString("x2"));
                    }
                    kh.MatKhau = builder.ToString();
                }
                kh.HoTen = sHoTen;
                kh.TaiKhoan = sTaiKhoan;
                kh.MatKhau = sMatKhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.DienThoai = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("DangNhap", "User");
            }
            return View("DangKy");
        }
        public ActionResult ThongTinCaNhan()
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
                return View(kh);
            }

            // Nếu người dùng chưa đăng nhập, bạn có thể xử lý theo ý muốn, ví dụ: chuyển hướng về trang đăng nhập.
            return RedirectToAction("DangNhap");
        }
    }
}