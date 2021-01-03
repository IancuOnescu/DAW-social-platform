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
        private EmailConfig Email = new EmailConfig();
 

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Message mes = db.Messages.Find(id);
            if (GroupAuth.IsAdminOrCreator(mes.GroupId, User.Identity.GetUserId()) || User.Identity.GetUserId() == mes.UserId || User.IsInRole("Admin"))
            {
                if (User.Identity.GetUserId() != mes.UserId)
                {
                    string author = mes.User.Email;
                    string notifBody = "<p>Ne pare rau, </p>";
                    notifBody += "<p>Unul dintre mesajele dumneavostra in grupul <b>" + mes.Group.GroupName + "</b> a fost sters de catre administrator. </p><br/>";
                    notifBody += "<p>Mesajul sters: </p>";
                    notifBody += "<p><b>" + mes.MessageContent + "</b></p><br/>";
                    notifBody += "<p>Va rugam sa fiti atent la continutul pe care il postati pe aceasta platforma.</p> <br/>";
                    notifBody += "<p>Echipa <b>DAW-social-app</b>.</p>";
                    Email.SendEmailNotification(author, "Mesajul Dvs. a fost sters!", notifBody);
                }

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
            if (GroupAuth.IsAdminOrCreator(mes.GroupId, User.Identity.GetUserId()) || User.Identity.GetUserId() == mes.UserId || User.IsInRole("Admin"))
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
                if (GroupAuth.IsAdminOrCreator(mes.GroupId, User.Identity.GetUserId()) || User.Identity.GetUserId() == mes.UserId || User.IsInRole("Admin"))
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