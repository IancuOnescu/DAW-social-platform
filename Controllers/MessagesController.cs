using DAW_social_platform.Infrastructure;
using DAW_social_platform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private GroupAuthorization GroupAuth = new GroupAuthorization();
 

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Message mes = db.Messages.Find(id);
            if (GroupAuth.IsAllowedToDelete(mes.GroupId, User.Identity.GetUserId()) || User.Identity.GetUserId() == mes.UserId || User.IsInRole("Admin"))
            {
                db.Messages.Remove(mes);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un mesaj care nu va apartine";
            }
            return Redirect("/Groups/Show/" + mes.GroupId);
        }

        public ActionResult Edit(int id)
        {
            Message mes = db.Messages.Find(id);
            if (GroupAuth.IsAllowedToEdit(mes.GroupId, User.Identity.GetUserId()) || User.Identity.GetUserId() == mes.UserId || User.IsInRole("Admin"))
            {
                ViewBag.message = mes;
                return View();
            } 
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati un mesaj care nu va apartine";
                return Redirect("/Groups/Show/" + mes.GroupId);
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Message requestMessage)
        {
            try
            {
                Message mes = db.Messages.Find(id);
                if (GroupAuth.IsAllowedToEdit(mes.GroupId, User.Identity.GetUserId()) || User.Identity.GetUserId() == mes.UserId || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(mes))
                    {
                        mes.MessageContent = requestMessage.MessageContent;
                        db.SaveChanges();
                    }
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa editati un mesaj care nu va apartine";
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