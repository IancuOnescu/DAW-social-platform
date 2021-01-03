using DAW_social_platform.Infrastructure;
using DAW_social_platform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    public class UsersController : Controller
    {
        private EmailConfig Email = new EmailConfig();
        private ApplicationDbContext db = ApplicationDbContext.Create();
        [Authorize(Roles = "User, Admin")]

        public ActionResult Index()
        {
            ViewBag.isAdmin = false;
            if (User.IsInRole("Admin"))
            {
                var users = from user in db.Users
                            orderby user.UserName
                            select user;
                ViewBag.UsersList = users;
                ViewBag.isAdmin = true;
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var users1 = from rel in db.UserRelationships
                             join usr in db.Users on rel.ToUserId equals usr.Id
                             where rel.FromUserId == userId && rel.RelationshipType == "accepted"
                             select usr;
                var users2 = from rel in db.UserRelationships
                             join usr in db.Users on rel.FromUserId equals usr.Id
                             where rel.ToUserId == userId && rel.RelationshipType == "accepted"
                             select usr;
                users1 = users1.Union(users2);
                ViewBag.UsersList = users1;
            }
            var profiles = db.Profiles.ToList();
            ViewBag.Profiles = profiles;
            return View();
        }
        [Authorize(Roles = "User, Admin")]
        [HttpPost]
        public ActionResult Add(string reqId)
        {
            var userId = User.Identity.GetUserId();
            var reqCheck = from rel in db.UserRelationships
                           where rel.ToUserId == userId || rel.FromUserId == userId
                           select rel;
            // Check if one of them has already sent a request
            if (reqCheck.Where(r => r.ToUserId == reqId).Count() == 0 && reqCheck.Where(r => r.FromUserId == reqId).Count() == 0)
            {
                try
                {
                    UserRelationship newRel = new UserRelationship();
                    newRel.ToUserId = reqId;
                    newRel.FromUserId = userId;
                    newRel.RelationshipType = "pending";
                    db.UserRelationships.Add(newRel);
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("SearchInfo", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        // GET: Show
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            string currentRole = user.Roles.FirstOrDefault().RoleId;
            var userRoleName = (from role in db.Roles
                                where role.Id == currentRole
                                select role.Name).First();
            ViewBag.roleName = userRoleName;
            return View(user);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
        [Authorize(Roles = "Admin, User")]
        public ActionResult Requests()
        {
            var usr = User.Identity.GetUserId();
            var req = db.UserRelationships.Where(a => a.ToUserId == usr && a.RelationshipType == "pending").Select(a => a.FromUserId).ToList();
            var reqUsers = db.Users.Where(a => req.Contains(a.Id));
            ViewBag.profiles = db.Profiles.ToList();
            ViewBag.requesters = reqUsers;
            return View();
        }
        [Authorize(Roles = "Admin, User")]
        public ActionResult AcceptFriendRequests(string fromId)
        {
            var usr = User.Identity.GetUserId();
            var req = db.UserRelationships.Where(a => a.ToUserId == usr && a.FromUserId == fromId).FirstOrDefault();

            UserRelationship newReq = new UserRelationship();
            newReq.ToUserId = req.ToUserId;
            newReq.FromUserId = req.FromUserId;
            newReq.RelationshipType = "accepted";

            db.UserRelationships.Add(newReq);
            db.UserRelationships.Remove(req);
            db.SaveChanges();

            return RedirectToAction("Requests");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                if (TryUpdateModel(user))
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                    user.PhoneNumber = newData.PhoneNumber;
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }
                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                newData.Id = id;
                return View(newData);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public ActionResult Delete(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);

            var profile = db.Profiles.FirstOrDefault(p => p.UserId == user.Id);

            if (!(profile is null))
            {
                db.Profiles.Remove(profile);
                db.SaveChanges();
            }

            string author = user.Email;
            string notifBody = "<p>Ne pare rau, </p>";
            notifBody += "<p>Contul Dvs. de utilizator a fost sters de catre un administrator :(</p>";
            notifBody += "<br/> <p>Echipa <b>DAW-social-app</b>.</p>";
            Email.SendEmailNotification(author, "Contul Dvs. a fost sters!", notifBody);

            UserManager.Delete(user);
            db.SaveChanges();
            return Redirect("/Account/AllUsers");
        }
    }
}