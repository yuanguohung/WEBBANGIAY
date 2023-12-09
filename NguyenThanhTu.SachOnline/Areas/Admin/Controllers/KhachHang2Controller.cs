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

    public class KhachHang2Controller : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/KhachHang
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 5;
            return View(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH).ToPagedList(iPageNum, iPageSize));
        }

        [HttpGet]
        public JsonResult DsKhachHang()
        {
            try
            {
                var dsKH = (from kh in db.KHACHHANGs
                            select new
                            {
                                MaKH = kh.MaKH,
                                TenKH = kh.HoTen,
                                TaiKhoan = kh.TaiKhoan,
                                MatKhau = kh.MatKhau,
                                Email = kh.Email,
                                NgaySinh = kh.NgaySinh,
                                DienThoai = kh.DienThoai,
                                DiaChi = kh.DiaChi

                            }).ToList();
                return Json(new { code = 200, dsCD = dsKH, msg = "Lấy danh sách khach hang thanh cong" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách khach hang thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Detail(int maKH)
        {
            try
            {
                var khachhang = (from s in db.KHACHHANGs
                            where (s.MaKH == maKH)
                            select new
                            {
                                MaKH = s.MaKH,
                                TenKH = s.HoTen,
                                TaiKhoan = s.TaiKhoan,
                                MatKhau = s.MatKhau,
                                Email = s.Email,
                                NgaySinh = s.NgaySinh,
                                DienThoai = s.DienThoai,
                                DiaChi = s.DiaChi

                            }).ToList();
                return Json(new { code = 200, khachhang = khachhang, msg = "Lấy chi tiết khách hàng thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy chi tiết khách hàng thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddKhachHang(string hoTen, string taiKhoan, string matKhau, string email, DateTime ngaySinh, string dienThoai, string diaChi)
        {
            try
            {
                var khachhang = new KHACHHANG();
                khachhang.HoTen = hoTen;
                khachhang.TaiKhoan = taiKhoan;
                khachhang.MatKhau = matKhau;
                khachhang.Email = email;
                khachhang.NgaySinh = ngaySinh;
                khachhang.DienThoai = dienThoai;
                khachhang.DiaChi = diaChi;

                db.KHACHHANGs.InsertOnSubmit(khachhang);
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Thêm khách hàng thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thêm khách hàng thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Update(int maKH, string hoTen, string taiKhoan, string matKhau, string email, DateTime ngaySinh, string dienThoai, string diaChi)
        {
            try
            {
                var khachhang = db.KHACHHANGs.SingleOrDefault(s => s.MaKH == maKH);

                if (khachhang != null)
                {
                    khachhang.HoTen = hoTen;
                    khachhang.TaiKhoan = taiKhoan;
                    khachhang.MatKhau = matKhau;
                    khachhang.Email = email;
                    khachhang.NgaySinh = ngaySinh;
                    khachhang.DienThoai = dienThoai;
                    khachhang.DiaChi = diaChi;

                    db.SubmitChanges();

                    return Json(new { code = 200, msg = "Cập nhật thông tin khách hàng thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 404, msg = "Không tìm thấy khách hàng cần cập nhật" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Cập nhật thông tin khách hàng thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int maKH)
        {
            try
            {
                var khachhang = db.KHACHHANGs.SingleOrDefault(k => k.MaKH == maKH);

                if (khachhang != null)
                {
                    db.KHACHHANGs.DeleteOnSubmit(khachhang);
                    db.SubmitChanges();

                    return Json(new { code = 200, msg = "Xóa khách hàng thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 404, msg = "Không tìm thấy khách hàng cần xóa" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa khách hàng thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}