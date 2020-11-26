using DAW_social_platform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Posts
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var posts = db.Posts.Include("User");
            ViewBag.posts = posts;
            return View();
        }

        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        public ActionResult Show(Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    return Redirect("/Posts/Show/" + comment.PostId);
                }

                else
                {
                    Post post = db.Posts.Find(comment.PostId);
                    return View(post);
                }

            }

            catch (Exception e)
            {
                Post post = db.Posts.Find(comment.PostId);
                return View(post);
            }
        }

        public ActionResult New()
        {
            Post post = new Post();
            post.UserId = User.Identity.GetUserId();
            return View();
        }

        [HttpPost]
        public ActionResult New(Post post)
        {
            post.Date = DateTime.Now;
            post.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    TempData["message"] = "Postarea a fost creata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(post);
                }
            }
            catch (Exception e)
            {
                return View(post);
            }
        }

        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        [HttpPut]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post post = db.Posts.Find(id);

                if (TryUpdateModel(post))
                {
                    post.Content = requestPost.Content;
                    db.SaveChanges();
                    TempData["message"] = "Postarea a fost modificata!";
                    return RedirectToAction("Index");
                }
                return View(requestPost);
            }
            catch (Exception e)
            {
                return View(requestPost);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            TempData["message"] = "Postarea a fost stearsa!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}