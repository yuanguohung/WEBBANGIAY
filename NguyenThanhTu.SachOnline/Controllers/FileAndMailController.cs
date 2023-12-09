using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenThanhTu.SachOnline.Models;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.IO;

namespace NguyenThanhTu.SachOnline.Controllers
{
    public class FileAndMailController : Controller
    {
        // GET: FileAndMail
        public ActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public ActionResult SendMail()
        {
            return View();

        }

        [HttpPost]
        public ActionResult SendMail(Mail model)
        {
            //cầu hình thông tìn gmail (khai bảo thứ viện system:Net) yar mail = new satpclient("smtp:gmail:com", 587)
            var mail = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("ntu38381@gmail.com", "xarkchhnozfrsblk"),
                // your email(abc @gmail.com) and your password()
                EnableSsl = true
            };
            //(khai bão thứ viện system.Net)

            //Tạo email

            var message = new MailMessage();
            message.From = new MailAddress(model.From);
            message.ReplyToList.Add(model.From);
            message.To.Add(new MailAddress(model.To));
            message.Subject = model.Subject;
            message.Body = model.Notes;

            var f = Request.Files["attachment"];
            var path = Path.Combine(Server.MapPath("~/UploadFile"), f.FileName);
            if (!System.IO.File.Exists(path))
            {
                f.SaveAs(path);
            }

            //(khai báo thư viện System.Net.Mime)

            Attachment data = new Attachment(Server.MapPath("~/UploadFile/" + f.FileName), MediaTypeNames.Application.Octet);
            message.Attachments.Add(data);

            //Gửi email

            mail.Send(message);
            return View("SendMail");
        }

    }
}