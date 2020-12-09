using DAW_social_platform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAW_social_platform.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profiles
        [Authorize(Roles = "User, Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.profiles = from Profile in db.Profiles
                              select Profile;
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            var userId = User.Identity.GetUserId();
            Profile profile = (from pr in db.Profiles
                               where pr.UserId == userId
                               select pr).FirstOrDefault();
            if (profile == null)
                return View();
            else return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult New(Profile profile)
        {
            try
            {
                var date = profile.BirthDate;
                profile.BirthDate = Convert.ToDateTime(date);
                profile.UserId = User.Identity.GetUserId();
                if (ModelState.IsValid)
                {
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost creat!";
                    return RedirectToAction("Index", "Manage");
                }
                else
                {
                    return View(profile);
                }
            }
            catch (Exception e)
            {
                return View(profile);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            var profile = db.Profiles.Find(id);
            if (profile != null)
            {
                if (profile.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    return View(profile);
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Profile profile = db.Profiles.Find(id);

                    if (profile.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(profile))
                        {
                            profile.Description = requestProfile.Description;
                            profile.BirthDate = Convert.ToDateTime(requestProfile.BirthDate);
                            profile.Hobbies = requestProfile.Hobbies;
                            profile.Status = requestProfile.Status;
                            profile.UserId = requestProfile.UserId;
                            db.SaveChanges();
                            TempData["message"] = "Profilul a fost modificat!";
                        }
                        return RedirectToAction("Index", "Manage");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return View(requestProfile);
                }
            }
            catch (Exception e)
            {
                return View(requestProfile);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Profiles.Remove(profile);
                TempData["message"] = "Profilul a fost sters!";
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un profil care nu va apartine";
                return RedirectToAction("Index");
            }
        }
    }
}