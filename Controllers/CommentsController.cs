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