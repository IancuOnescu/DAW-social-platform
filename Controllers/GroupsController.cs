using DAW_social_platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Groups
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.groups = from Group in db.Groups
                             select Group;
            return View();
        }

        public ActionResult Show(int id)
        {
            Group group = db.Groups.Find(id);
            return View(group);
        }

        [HttpPost]
        public ActionResult Show(Message message)
        {
            message.date = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Messages.Add(message);
                    db.SaveChanges();
                    return Redirect("/Groups/Show/" + message.groupId);
                }

                else
                {
                    Group group = db.Groups.Find(message.groupId);
                    return View(group);
                }

            }

            catch (Exception e)
            {
                Group group = db.Groups.Find(message.groupId);
                return View(group);
            }
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Group group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(group);
                    db.SaveChanges();
                    TempData["message"] = "Grupul a fost creat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(group);
                }
            }
            catch (Exception e)
            {
                return View(group);
            }
        }

        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            return View(group);
        }

        [HttpPut]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                Group group = db.Groups.Find(id);

                if (TryUpdateModel(group))
                {
                    group.groupName = requestGroup.groupName;
                    db.SaveChanges();
                    TempData["message"] = "Grupul a fost modificat!";
                    return RedirectToAction("Index");
                }
                return View(requestGroup);
            }
            catch (Exception e)
            {
                return View(requestGroup);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            TempData["message"] = "Grupul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}