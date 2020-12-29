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
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private EmailConfig Email = new EmailConfig();

        // GET: Comments
        [Authorize(Roles = "User,Visitor,Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                if (User.IsInRole("Admin") && comment.UserId != User.Identity.GetUserId())
                {
                    string author = comment.User.Email;

                    string notifBody = "<p>Unul dintre comentariile dumneavostra a fost sters de catre administrator. </p><br/>";
                    notifBody += "<p>Postarea: </p>";
                    notifBody += "<p><b>" + comment.Post.Content + "</b></p><br />";
                    notifBody += "<p>Comentariul sters: </p>";
                    notifBody += "<p><b>" + comment.Content + "</b></p><br/><br/>";
                    notifBody += "<p>Va rugam sa fiti atent la continutul pe care il postati pe aceasta platforma.</p>";
                    notifBody += "<br/> <p>Echipa <b>DAW-social-app</b></p>";

                    Email.SendEmailNotification(author, "Comentariu sters!", notifBody);
                }

                db.Comments.Remove(comment);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comentariu care nu va apartine";
            }
            return Redirect("/Posts/Show/" + comment.PostId);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.comment = comment;
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine";
                return Redirect("/Posts/Show/" + comment.PostId);
            }
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(comment))
                    {
                        comment.Content = requestComment.Content;
                        comment.Date = DateTime.Now;
                        comment.UserId = User.Identity.GetUserId();
                        db.SaveChanges();
                    }
                    return Redirect("/Posts/Show/" + comment.PostId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra uneri postari care nu va apartine";
                    return Redirect("/Posts/Show/" + comment.PostId);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}