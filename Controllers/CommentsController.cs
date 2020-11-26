using DAW_social_platform.Models;
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
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return Redirect("/Posts/Show/" + comment.PostId);
        }

        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            ViewBag.comment = comment;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (TryUpdateModel(comment))
                {
                    comment.Content = requestComment.Content;
                    db.SaveChanges();
                }
                return Redirect("/Posts/Show/" + comment.PostId);
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}