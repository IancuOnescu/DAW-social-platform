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
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private GroupAuthorization GroupAuth = new GroupAuthorization();

        // GET: Groups
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var creatorRoleId = (from r in db.GroupRoles
                            where r.RoleName == "Creator"
                            select r.RoleId).FirstOrDefault();
            var adminRoleId = (from r in db.GroupRoles
                             where r.RoleName == "Admin"
                             select r.RoleId).FirstOrDefault();
            var userRoleId = (from r in db.GroupRoles
                           where r.RoleName == "User"
                           select r.RoleId).FirstOrDefault();
            var currentUserId = User.Identity.GetUserId();

            ViewBag.creatorGroups = (from g in db.Groups
                                     join gu in db.GroupUsers on g.GroupId equals gu.GroupId
                                     where gu.UserId == currentUserId && gu.RoleId == creatorRoleId
                                     select g).ToList();
            ViewBag.adminGroups = (from g in db.Groups
                                  join gu in db.GroupUsers on g.GroupId equals gu.GroupId
                                  where gu.UserId == currentUserId && gu.RoleId == adminRoleId
                                  select g).ToList();
            ViewBag.userGroups = (from g in db.Groups
                                 join gu in db.GroupUsers on g.GroupId equals gu.GroupId
                                  where gu.UserId == currentUserId && gu.RoleId == userRoleId
                                  select g).ToList();
            var isAdmin = User.IsInRole("Admin");
            ViewBag.isAdmin = isAdmin;
            if (isAdmin)
            {
                ViewBag.allGroups = db.Groups.ToList();
            }

            return View();
        }

        public ActionResult Show(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            if (GroupAuth.IsUserOrAdminOrCreator(id, currentUserId) || User.IsInRole("Admin"))
            {
                Group group = db.Groups.Find(id);

                ViewBag.isGroupAdminCreatorOrAppAdmin = false;
                ViewBag.showEditButton = false;
                if (GroupAuth.IsAdminOrCreator(id, currentUserId) || User.IsInRole("Admin"))
                {
                    ViewBag.showEditButton = true;
                    ViewBag.isGroupAdminCreatorOrAppAdmin = true;
                }
                ViewBag.showDeleteButton = false;
                if (GroupAuth.IsCreator(id, currentUserId) || User.IsInRole("Admin"))
                {
                    ViewBag.showDeleteButton = true;
                }
                ViewBag.currentUserId = currentUserId;

                return View(group);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Show(Message message)
        {
            if (GroupAuth.IsUserOrAdminOrCreator(message.GroupId, User.Identity.GetUserId()) || User.IsInRole("Admin"))
            {
                message.Date = DateTime.Now;
                message.UserId = User.Identity.GetUserId();
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Messages.Add(message);
                        db.SaveChanges();
                        return Redirect("/Groups/Show/" + message.GroupId);
                    }
                    else
                    {
                        Group group = db.Groups.Find(message.GroupId);
                        return View(group);
                    }

                }

                catch (Exception e)
                {
                    Group group = db.Groups.Find(message.GroupId);
                    return View(group);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Group reqGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Verificare grupuri cu acelasi nume

                    reqGroup.UserId = User.Identity.GetUserId();
                    db.Groups.Add(reqGroup);
                    db.SaveChanges();

                    AddUserToGroupRole("Creator", reqGroup.GroupName);

                    TempData["message"] = "Grupul a fost creat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View (reqGroup);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return View(reqGroup);
            }
        }

        public ActionResult Edit(int id)
        {
            if (GroupAuth.IsAdminOrCreator(id, User.Identity.GetUserId()) || User.IsInRole("Admin"))
            {
                Group group = db.Groups.Find(id);
                return View(group);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Group requestGroup)
        {
            if (GroupAuth.IsAdminOrCreator(id, User.Identity.GetUserId()) || User.IsInRole("Admin"))
            {
                try
                {
                    Group group = db.Groups.Find(id);

                    if (TryUpdateModel(group))
                    {
                        group.GroupName = requestGroup.GroupName;
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
            else
            {
                TempData["message"] = "Nu aveti drepturi suficiente pentru a modifica acest grup!";
                return RedirectToAction("Index");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            if (GroupAuth.IsCreator(id, User.Identity.GetUserId()) || User.IsInRole("Admin"))
            {
                Group group = db.Groups.Find(id);
                db.Groups.Remove(group);
                TempData["message"] = "Grupul a fost sters!";
                db.SaveChanges();
            } 
            else
            {
                TempData["message"] = "Nu aveti drepturi suficiente pentru a sterge acest grup!";
            }
            return RedirectToAction("Index");
        }

        [NonAction]
        public void AddUserToGroupRole(string role, string name)
        {
            string userId = User.Identity.GetUserId();

            var newRoleId = (from gr in db.GroupRoles
                             where gr.RoleName == role
                             select new
                             { gr.RoleId }).FirstOrDefault();

            var newGroupId = (from g in db.Groups
                              where g.UserId == userId && g.GroupName == name
                              select new
                              { g.GroupId }).FirstOrDefault();

            GroupUsers groupUser = new GroupUsers();
            groupUser.RoleId = newRoleId.RoleId;
            groupUser.GroupId = newGroupId.GroupId;
            groupUser.UserId = userId;

            db.GroupUsers.Add(groupUser);
            db.SaveChanges();
        }
    }
}