using DAW_social_platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Messages
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Message mes = db.Messages.Find(id);
            db.Messages.Remove(mes);
            db.SaveChanges();
            return Redirect("/Groups/Show/" + mes.GroupId);
        }

        public ActionResult Edit(int id)
        {
            Message mes = db.Messages.Find(id);
            ViewBag.message = mes;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Message requestMessage)
        {
            try
            {
                Message mes = db.Messages.Find(id);
                if (TryUpdateModel(mes))
                {
                    mes.MessageContent = requestMessage.MessageContent;
                    db.SaveChanges();
                }
                return Redirect("/Groups/Show/" + mes.GroupId);
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}