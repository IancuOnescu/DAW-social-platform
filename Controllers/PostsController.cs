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
        [Authorize(Roles = "User,Admin")]
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

        [Authorize(Roles = "User,Admin")]
        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);

            ViewBag.afisareButoane = false;
            if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(post);
        }

        [Authorize(Roles = "User,Visitor,Admin")]
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

        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            Post post = new Post();
            post.UserId = User.Identity.GetUserId();
            return View();
        }

        [Authorize(Roles = "User,Admin")]
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

        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            post.Date = DateTime.Now;
            post.UserId = User.Identity.GetUserId();

            if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine";
                return RedirectToAction("Index");
            }

            return View(post);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Post post = db.Posts.Find(id);

                    if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(post))
                        {
                            post.Content = requestPost.Content;
                            post.Date = DateTime.Now;
                            post.UserId = User.Identity.GetUserId();
                            db.SaveChanges();
                            TempData["message"] = "Postarea a fost modificata!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra uneri postari care nu va apartine";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestPost);
                }
            }
            catch (Exception e)
            {
                return View(requestPost);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            if (post.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Posts.Remove(post);
                TempData["message"] = "Postarea a fost stearsa!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra uneri postari care nu va apartine";
                return RedirectToAction("Index");
            }
        }
    }
}