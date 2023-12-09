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

    public class DonHangController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/DonHang
        public ActionResult Index(int ? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(iPageNum, iPageSize));
        }
    }

}