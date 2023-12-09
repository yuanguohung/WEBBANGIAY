using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
namespace NguyenThanhTu.SachOnline.Areas.Admin.Controllers
{

    public class KhachHangController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/KhachHang
        public ActionResult Index(int ? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 5;
            return View(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH).ToPagedList(iPageNum, iPageSize));
        }
        public ActionResult Details(int id)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                Response.StatusCode = 400;
                return null;
            }
            return View(kh);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            return View(kh);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection f)
        {
            // Chuyển đổi giá trị iMaCD từ FormCollection sang kiểu int
            int maKH = int.Parse(f["iMaCD"]);

            // Tìm khách hàng dựa trên MaKH
            var kh = db.KHACHHANGs.FirstOrDefault(n => n.MaKH == maKH);

            if (kh != null)
            {
                // Cập nhật thông tin khách hàng từ dữ liệu FormCollection
                kh.HoTen = f["sHoTen"];
                kh.TaiKhoan = f["sTaiKhoan"];
                kh.MatKhau = f["sMatKhau"];
                kh.Email = f["sEmail"];
                kh.NgaySinh = Convert.ToDateTime(f["dNgaySinh"]);
                kh.DienThoai = f["iDienThoai"];
                kh.DiaChi = f["fDiaChi"];

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý khi không tìm thấy khách hàng với MaKH tương ứng
                // Ví dụ: Chuyển hướng đến một trang lỗi hoặc hiển thị thông báo lỗi
                ViewBag.ErrorMessage = "Không tìm thấy khách hàng với MaKH tương ứng.";
                return View("Error"); // Chuyển hướng đến trang lỗi hoặc thay đổi tùy ý
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var check = db.DONDATHANGs.Where(ct => ct.MaKH == id);
            if (check.Count() > 0)
            {
                ViewBag.ThongBao = "Khách hàng này hiện có trong đơn đặt hàng <br>" + "Nếu muốn xóa thì phải xóa hết MaKH này trong DONDATHANG";
                return View(kh);
            }
            db.KHACHHANGs.DeleteOnSubmit(kh);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }


    }
}