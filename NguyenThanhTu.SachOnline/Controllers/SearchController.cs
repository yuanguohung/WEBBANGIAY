using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.Linq.Dynamic;
using System.Linq.Expressions;
namespace NguyenThanhTu.SachOnline.Controllers
{
    public class SearchController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public ActionResult Search(string strSearch=null)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                var kq = from s in db.SACHes where s.TenSach.Contains(strSearch) || s.CHUDE.TenChuDe.Contains(strSearch) || s.NHAXUATBAN.TenNXB.Contains(strSearch) || s.MoTa.Contains(strSearch) select s;
                return View(kq);
            }
            return View();
        }
        public ActionResult SearchTheoDanhMuc(string strSearch=null, int maCD =0)
        {
            ViewBag.Search = strSearch;
            var kq = db.SACHes.Select(b => b);
            if (!String.IsNullOrEmpty(strSearch))
            {
                kq = kq.Where(b => b.TenSach.Contains(strSearch));

            }
            if (maCD != 0)
            {
                kq = kq.Where(b => b.CHUDE.MaCD == maCD);
            }

            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe");
            return View(kq.ToList());
        }
        public ActionResult Group()
        {
            var kq = db.SACHes.GroupBy(s => s.MaCD);

            return View(kq);
        }
        public ActionResult ThongKe()
        {
            var kq = from s in db.SACHes
                     join cd in db.CHUDEs on s.MaCD equals cd.MaCD
                     group s by new { cd.MaCD, cd.TenChuDe } into g
                     select new ReportInfo
                     {
                         Id = g.Key.MaCD.ToString(),
                         Name = g.Key.TenChuDe,
                         Count = g.Count(),
                         Sum = g.Sum(n => n.SoLuongBan),
                         Max = g.Max(n => n.SoLuongBan),
                         Min = g.Min(n => n.SoLuongBan),
                         Avg = Convert.ToDecimal(g.Average(n => n.SoLuongBan))
                     };
            return View(kq);
        }
        public ActionResult SearchPhanTrang(int ? page, string strSearch = null)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                int iSize = 3;
                int iPageNumber = (page ?? 1);
                var kq = from s in db.SACHes where s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch) select s;
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            return View();
        }
        public ActionResult SearchPhanTrangTuyChon(int? size, int? page, string strSearch = null)
        {
            //1 List để lấy nguồn cho Combobox chọn số lượng sản phẩm
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            //Giữ trạng thái kích thước trang được chọn trên DropDrownList
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.size = items;
            ViewBag.currentSize = size;
            ViewBag.Search = strSearch;
            int iSize = (size ?? 3);
            int iPageNumber = (page ?? 1);
            var kq = from s in db.SACHes select s;
            if (!string.IsNullOrEmpty(strSearch))
            {
                kq = kq.Where(s => s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch));

            }
            return View(kq.ToPagedList(iPageNumber, iSize));

        }
        public ActionResult SearchPhanTrangSapXep(int? page, string sortProperty, string sortOrder = "", string strSearch = null)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                int iSize = 3;
                int iPageNumber = (page ?? 1);
                // Gián giá trị cho biến sortOrder
                if (sortOrder == "") ViewBag.SortOrder = "desc";
                if (sortOrder == "desc") ViewBag.SortOrder = "";
                if (sortOrder == "") ViewBag.SortOrder = "asc";
                // Tạo thuộc tính sắp xếp mặc định là " Tên sách "
                if (String.IsNullOrEmpty(sortProperty))       
                    sortProperty = "TenSach";
                // Gián giá trị cho biến sortProperty
                ViewBag.SortProperty = sortProperty;
                // Truy vấn
                var kq = from s in db.SACHes where s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch) select s;

                //Sắp xếp tăng/ giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
                if (sortOrder == "desc")
                {
                    kq = kq.OrderBy(sortProperty + " desc");
                }
                else
                {
                    kq = kq.OrderBy(sortProperty);
                }
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            return View();
        }

    }
}