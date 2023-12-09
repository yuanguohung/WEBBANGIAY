using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using System.Net;
using System.Net.Mail;
namespace NguyenThanhTu.SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if(lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int ms, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iMaSach == ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);
            }
            else
            {
                sp.iSoLuong++;

            }
            return Redirect(url);
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);

            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dThanhTien);

            }
            return dTongTien;
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult XoaSPKhoiGioHang(int iMaSach)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSach);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnline");
                }
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapNhatGioHang(int iMaSach, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if(Session["TaiKhoan"]==null||Session["TaiKhoan"].ToString()=="")
            {
                return RedirectToAction("DangNhap", "User", new { returnUrl = Request.Url.PathAndQuery });

            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG taiKhoanKH = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> lstGioHang = LayGioHang();
            ddh.MaKH = taiKhoanKH.MaKH;
            ddh.NgayDat = DateTime.Now;

            // Kiểm tra ngày giao
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            DateTime ngayGiao;
            if (!DateTime.TryParse(NgayGiao, out ngayGiao) || ngayGiao < DateTime.Now)
            {
                ModelState.AddModelError("NgayGiao", "Ngày giao không hợp lệ.");
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                return View("DatHang", lstGioHang);
            }
            else if (ngayGiao <= ddh.NgayDat) // Kiểm tra ngày giao trước ngày đặt hàng
            {
                ModelState.AddModelError("NgayGiao", "Ngày giao phải sau ngày đặt hàng.");
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                return View("DatHang", lstGioHang);
            }
            try
            {
                var Total = TongTien();
                string subject = "Đơn hàng của bạn đã được đặt thành công";
                string body = "Cảm ơn bạn đã đặt hàng! Đơn hàng của bạn đã được xác nhận." + "<br /><br />";
                body += "Khách hàng:" + taiKhoanKH.HoTen + "<br />";
                body += "Địa chỉ:" + taiKhoanKH.DiaChi + "<br />";
                body += "Số điện thoại:" + taiKhoanKH.DienThoai + "<br /><br />";
                body += "Thông tin chi tiết đơn hàng:" + "<br />";
                body += "Ngày đặt hàng: " + ddh.NgayDat.ToString() + "<br />";
                body += "Ngày giao hàng: " + ngayGiao + "<br />";

                // Lấy thông tin chi tiết sản phẩm đã đặt
                body += "Các sản phẩm đã đặt:" + "<br />";
                foreach (var item in lstGioHang)
                {
                    body += "Tên sách: " + item.sTenSach + "<br />";
                    body += "Số lượng: " + item.iSoLuong + "<br />";
                    body += "Đơn giá: " + item.dDonGia.ToString("C") + "<br />"; // Định dạng tiền tệ
                    body += "<br />";
                }
                body += "Tổng tiền tất cả sản phẩm vừa đặt:" + Total + "<br />";
                string toEmail = taiKhoanKH.Email;

                // Gửi email
                SendEmail(toEmail, subject, body);

                // ... (phần code khác)
            }
            catch (SmtpException ex)
            {
                ViewBag.ErrorMessage = "Có lỗi khi gửi email. Vui lòng thử lại sau.";
                return View("DatHang", lstGioHang);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View("DatHang", lstGioHang);
            }
            ddh.NgayGiao = ngayGiao;
            ddh.TinhTrangGiaoHang = 1;
            db.DONDATHANGs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            // Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMaSach;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                db.CHITIETDATHANGs.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        private void SendEmail(string to, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("thanhtu1233210@gmail.com", "SachOnline");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                {
                    smtp.Port = 587;
                    smtp.Credentials = new NetworkCredential("thanhtu1233210@gmail.com", "hhqa vclq bpnd abbn");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }


        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        // GET: GioHang
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if(lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
    }
}