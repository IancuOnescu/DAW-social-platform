using DAW_social_platform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult SearchInfo()
        {
            var usr = User.Identity.GetUserId();
            var users = db.Users.OrderBy(a => a.UserName).ToList();
            var profiles = db.Profiles.ToList();
            var groups = db.Groups.OrderBy(a => a.GroupName).ToList();
            var search = "";

            if (Request.Params.Get("search") != null && Request.Params.Get("search") == "")
                return Redirect("Index");

            if(Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<string> userIds = db.Users.Where(
                    un => un.UserName.Contains(search)
                    ).Select(a => a.Id).ToList();
                List<int> groupIds = db.Groups.Where(
                    gn => gn.GroupName.Contains(search)
                    || gn.Description.Contains(search)
                    ).Select(a => a.GroupId).ToList();

                users = db.Users.Where(usrs => userIds.Contains(usrs.Id)).OrderBy(a => a.UserName).ToList();
                groups = db.Groups.Where(grp => groupIds.Contains(grp.GroupId)).OrderBy(a => a.GroupName).ToList();
            }
            
            var totalItems = users.Count() + groups.Count();
            var maxItems = users.Count();
            if (groups.Count() > maxItems)
                maxItems = groups.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;
            var pageSize = 3;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * pageSize;
            }

            var paginatedUsers = users.Skip(offset).Take(pageSize);
            var paginatedGroups = groups.Skip(offset).Take(pageSize);
            
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            List<string> notAddable1 = db.UserRelationships.Where(a => a.ToUserId == usr).Select(a => a.FromUserId).ToList();
            List<string> notAddable2 = db.UserRelationships.Where(a => a.FromUserId == usr).Select(a => a.ToUserId).ToList();
            var addable = db.Users.Where(a => !notAddable1.Contains(a.Id) && !notAddable2.Contains(a.Id) && a.Id != usr).ToList();

            ViewBag.total = totalItems;
            ViewBag.lastpage = Math.Ceiling((float)maxItems / (float)pageSize);
            ViewBag.Users = paginatedUsers.ToList();
            ViewBag.Groups = paginatedGroups.ToList();
            ViewBag.Profiles = profiles;
            ViewBag.Addable = addable;
            ViewBag.usr = usr;
            ViewBag.SearchString = search;
            
            return View();
        }
    }
}