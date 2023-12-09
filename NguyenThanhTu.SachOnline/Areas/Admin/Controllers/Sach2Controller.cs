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
    public class Sach2Controller : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/Sach2
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.SACHes.ToList().OrderBy(n => n.MaSach).ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public JsonResult DsSach()
        {
            try
            {
                var dssach = (from sach in db.SACHes
                            select new
                            {
                                MaSach = sach.MaSach,
                                TenSach = sach.TenSach,
                                MoTa = sach.MoTa,
                                AnhBia = sach.AnhBia,
                                NgayCapNhat = sach.NgayCapNhat,
                                SoLuong = sach.SoLuongBan,
                                GiaBan = sach.GiaBan,
                                MaCD = sach.MaCD,
                                NXB = sach.MaNXB

                            }).ToList();
                return Json(new { code = 200, dssach=dssach, msg = "Lấy danh sách sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách sách thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult Detail(int maSach)
        {
            try
            {
                var sach = (from s in db.SACHes
                              where (s.MaSach == maSach)
                              select new
                              {
                                  MaSach = s.MaSach,
                                  TenSach = s.TenSach,
                                  MoTa = s.MoTa,
                                  AnhBia = s.AnhBia,
                                  NgayCapNhat = s.NgayCapNhat,
                                  SoLuong = s.SoLuongBan,
                                  GiaBan = s.GiaBan,
                                  MaCD = s.MaCD,
                                  NXB = s.MaNXB

                              }).ToList();
                return Json(new { code = 200, sach = sach, msg = "Lấy chi tiết sách thành công" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy chi tiết sách thất bại" + ex.Message },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddSach(string sTenSach, string sMoTa, int iSoLuong, decimal mGiaBan, int MaCD, int MaNXB, DateTime dNgayCapNhat, HttpPostedFileBase fFileUpload)
        {
            try
            {
                if (fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }

                    var sach = new SACH
                    {
                        TenSach = sTenSach,
                        MoTa = sMoTa,
                        AnhBia = sFileName,
                        NgayCapNhat = dNgayCapNhat,
                        SoLuongBan = iSoLuong,
                        GiaBan = mGiaBan,
                        MaCD = MaCD,
                        MaNXB = MaNXB
                    };

                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();

                    return Json(new { code = 200, msg = "Thêm sách thành công" });
                }
                else
                {
                    return Json(new { code = 400, msg = "Hãy chọn ảnh bìa" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thêm sách thất bại. Lỗi " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult Update(int maSach, string sTenSach, string sMoTa, int iSoLuong, decimal mGiaBan, int MaCD, int MaNXB, DateTime dNgayCapNhat, HttpPostedFileBase fFileUpload)
        {
            try
            {
                var sach = db.SACHes.SingleOrDefault(n => n.MaSach == maSach);
                if (sach == null)
                {
                    return Json(new { code = 404, msg = "Không tìm thấy sách" }, JsonRequestBehavior.AllowGet);
                }

                if (fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }

                    sach.AnhBia = sFileName;
                }

                sach.TenSach = sTenSach;
                sach.MoTa = sMoTa;
                sach.NgayCapNhat = dNgayCapNhat;
                sach.SoLuongBan = iSoLuong;
                sach.GiaBan = mGiaBan;
                sach.MaCD = MaCD;
                sach.MaNXB = MaNXB;

                db.SubmitChanges();

                return Json(new { code = 200, msg = "Sửa sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Sửa sách thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int maSach)
        {
            try
            {
                var sach = db.SACHes.SingleOrDefault(n => n.MaSach == maSach);
                if (sach == null)
                {
                    return Json(new { code = 404, msg = "Không tìm thấy sách" }, JsonRequestBehavior.AllowGet);
                }

                var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaSach == maSach);
                if (ctdh.Any())
                {
                    return Json(new { code = 400, msg = "Sách này đang có trong bảng chi tiết đặt hàng. Hãy xóa hết mã sách này trong bảng chi tiết đặt hàng trước" }, JsonRequestBehavior.AllowGet);
                }

                var vietsach = db.VIETSACHes.Where(vs => vs.MaSach == maSach).ToList();
                if (vietsach.Count > 0)
                {
                    db.VIETSACHes.DeleteAllOnSubmit(vietsach);
                }

                db.SACHes.DeleteOnSubmit(sach);
                db.SubmitChanges();

                return Json(new { code = 200, msg = "Xóa sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa sách thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}