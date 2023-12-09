using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;

namespace NguyenThanhTu.SachOnline.Areas.Admin.Controllers
{

    public class NhaXuatBanController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/NhaXuatBan
        public ActionResult Index()
        {
            return View(db.NHAXUATBANs);
        }
        public ActionResult ChiTiet()
        {
            int manxb = int.Parse(Request.QueryString["id"]);
            return View(GetNXB(manxb));
        }
        public NHAXUATBAN GetNXB(int id)
        {
            return db.NHAXUATBANs.Where(nxb => nxb.MaNXB == id).SingleOrDefault();
        }

        public ActionResult XoaNXB(int id)
        {
            var nxb = GetNXB(id);

            if (nxb != null)
            {
                db.NHAXUATBANs.DeleteOnSubmit(nxb);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ThemNXB()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNXB(NHAXUATBAN nhaXuatBan)
        {
            if (ModelState.IsValid)
            {
                db.NHAXUATBANs.InsertOnSubmit(nhaXuatBan);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            return View(nhaXuatBan);

        }

        [HttpGet]
        public ActionResult CapNhatNXB(int id)
        {
            return View(GetNXB(id));
        }
        [HttpPost]
        public ActionResult CapNhatNXB(FormCollection f)
        {
            var nxb = GetNXB(int.Parse(f["MaNXB"]));
            nxb.TenNXB = f["TenNXB"];
            nxb.DiaChi = f["DiaChi"];
            nxb.DienThoai = f["DienThoai"];
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

    }
}