using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
namespace NguyenThanhTu.SachOnline.Areas.Admin.Controllers
{
    public class ChuDeController : Controller
    {
        // GET: Admin/ChuDe
        DataClasses1DataContext db = new DataClasses1DataContext();
        public ActionResult Index()
        {
            return View(db.CHUDEs);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var cd = db.CHUDEs.SingleOrDefault(n => n.MaCD == id);
            if (cd == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cd);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var cd = db.CHUDEs.SingleOrDefault(n => n.MaCD == id);
            if (cd == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var check = db.SACHes.Where(ct => ct.MaCD == id);
            if (check.Count() > 0)
            {
                ViewBag.ThongBao = "Chủ đề này đang có trong bảng Sach <br>" + "Nếu muốn xóa thì phải xóa hết mã chủ đề này trong bảng Sach";
                return View(cd);
            }

            db.CHUDEs.DeleteOnSubmit(cd);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CHUDE newChuDe)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem chủ đề đã tồn tại hay chưa
                var existingChuDe = db.CHUDEs.SingleOrDefault(n => n.TenChuDe == newChuDe.TenChuDe);
                if (existingChuDe != null)
                {
                    ModelState.AddModelError("TenCD", "Chủ đề đã tồn tại.");
                    return View(newChuDe);
                }

                // Lưu chủ đề mới vào cơ sở dữ liệu
                db.CHUDEs.InsertOnSubmit(newChuDe);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }

            return View(newChuDe);
        }

    }
}