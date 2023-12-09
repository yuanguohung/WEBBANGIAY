using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using System.Text;
using System.Globalization;
namespace NguyenThanhTu.SachOnline.Areas.Admin.Controllers
{
    public class TrangTinController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/TrangTin
        public ActionResult Index()
        {
            return View(db.TRANGTINs.ToList());
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create (TRANGTIN tt)
        {
            if (ModelState.IsValid)
            {
                tt.MetaTitle = RemoveDiacritics(tt.TenTrang).Replace(" ", "-");
                tt.NgayTao = DateTime.Now;
                db.TRANGTINs.InsertOnSubmit(tt);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public string RemoveDiacritics(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var tt = db.TRANGTINs.Where(t => t.MaTT == id);
            return View(tt.SingleOrDefault());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            if (ModelState.IsValid)
            {
                var tt = db.TRANGTINs.Where(t => t.MaTT == int.Parse(f["MaTT"])).SingleOrDefault();
                tt.TenTrang = f["TenTrang"];
                tt.NoiDung = f["NoiDung"];
                tt.NgayTao = Convert.ToDateTime(f["NgayTao"]);
                tt.MetaTitle = RemoveDiacritics(f["TenTrang"]).Replace(" ", "-");
                db.SubmitChanges();
                return RedirectToAction("Index");


            }
            else
            {
                return RedirectToAction("Edit");
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var tt = from t in db.TRANGTINs where t.MaTT == id select t;
            return View(tt.SingleOrDefault());
        }
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var tt = (from t in db.TRANGTINs where t.MaTT == id select t).SingleOrDefault();
            db.TRANGTINs.DeleteOnSubmit(tt);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

    }
}