using DAW_social_platform.Models;
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
            var users = db.Users.OrderBy(a => a.UserName);
            var groups = db.Groups.OrderBy(a => a.GroupName);
            var search = "";
            
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

                users = db.Users.Where(usr => userIds.Contains(usr.Id)).OrderBy(a => a.UserName);
                groups = db.Groups.Where(grp => groupIds.Contains(grp.GroupId)).OrderBy(a => a.GroupName);
            }
            
            var totalItems = users.Count() + groups.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;
            var pageSize = 2;

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

            ViewBag.total = totalItems;
            ViewBag.lastpage = Math.Ceiling((float)totalItems / (float)pageSize);
            ViewBag.Users = paginatedUsers;
            ViewBag.Groups = paginatedGroups;
            ViewBag.SearchString = search;
            
            return View();
        }
    }
}