using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using PagedList;
using PagedList.Mvc;

namespace NguyenThanhTu.SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: SachOnline
        private List<SACH> LaySachMoi(int count)
        {
            return db.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();

        }
        private List<SACH> LaySachBanNhieu(int count)
        {
            return db.SACHes.OrderByDescending(a => a.SoLuongBan).Take(count).ToList();
        }

        public ActionResult Index(int ? page)
        {
            var listSachMoi = LaySachMoi(20);
            var maCD = Request.QueryString["MaCD"];
            ViewBag.MaCD = maCD;
            int iSize = 6;
            int iPageNumber = (page ?? 1);

            return View(listSachMoi.ToPagedList(iPageNumber, iSize));
        }
        [ChildActionOnly]
        public ActionResult SachBanNhieuPartial()
        {
            var listSachBN = LaySachBanNhieu(4);
            return View(listSachBN);
        }
        [ChildActionOnly]
        public ActionResult ChuDePartial()
        {
            var cd = from c in db.CHUDEs select c;
            return PartialView(cd);
        }
        [ChildActionOnly]
        public ActionResult NhaXuatBanPartial()
        {
            var nxb = from c in db.NHAXUATBANs select c;
            return PartialView(nxb);
        }
        
        [ChildActionOnly]
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public ActionResult NavPartial()
        {
            List<MENU> lst = new List<MENU>();
            lst = db.MENUs.Where(m => m.ParentId == null).OrderBy(m => m.OrderNumber).ToList();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                var l = db.MENUs.Where(m => m.ParentId == lst[i].Id);
                a[i] = l.Count();
            }
            ViewBag.lst = a;
            return PartialView(lst);
        }
        [ChildActionOnly]
        public ActionResult LoadChildMenu(int parentId)
        {
            List<MENU> lst = new List<MENU>();
            lst = db.MENUs.Where(m => m.ParentId == parentId).OrderBy(m => m.OrderNumber).ToList();
            ViewBag.Count = lst.Count();
            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                var l = db.MENUs.Where(m => m.ParentId == lst[i].Id);
                a[i] = l.Count();
            }
            ViewBag.lst = a;
            return PartialView("LoadChildMenu", lst);
        }
        public ActionResult TrangTin(string metatitle)
        {
            var tt = (from t in db.TRANGTINs where t.MetaTitle == metatitle select t).Single();
            return View(tt);
        }
        [ChildActionOnly]
        public ActionResult SliderPartial()
        {
            return PartialView();
        }
        public ActionResult LoginLogoutPartial()
        {
            return PartialView();
        }
        public ActionResult ChiTietSach(int id)
        {
            var sach = from s in db.SACHes
                       where s.MaSach == id
                       select s;
            return View(sach.Single());
        }
        public ActionResult SachTheoChuDe(int? page, int MaCD)
        {

            if (MaCD != null)
            {
                ViewBag.MaCD = MaCD;
                int iSize = 3;
                int iPageNumber = (page ?? 1);

                var kq = from s in db.SACHes where s.MaCD == MaCD select s;
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            else
            {
                // Xử lý trường hợp MaCD là null hoặc rỗng
                // Ví dụ: return một trang thông báo lỗi hoặc redirect về trang khác.
                return RedirectToAction("Index"); // Ví dụ redirect về trang "Index"
            }
        }
        public ActionResult SachTheoNhaXuatBan(int? page, int MaNXB)
        {
            ViewBag.MaNXB = MaNXB;
            int iSize = 3;
            int iPageNumber = (page ?? 1);
            var kq = from s in db.SACHes where s.MaNXB == MaNXB select s;
            return View(kq.ToPagedList(iPageNumber, iSize));
        }

    }
}