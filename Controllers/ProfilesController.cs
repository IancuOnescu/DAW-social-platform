﻿using DAW_social_platform.Models;
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

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Profile profile)
        {
            try
            {
                var date = profile.birthDate;
                profile.birthDate = Convert.ToDateTime(date);
                profile.profileStatus = "Public";
                if (ModelState.IsValid)
                {
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost creat!";
                    return RedirectToAction("Index");
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

        public ActionResult Edit(int id)
        {
            var profile = db.Profiles.Find(id);
            return View(profile);
        }

        [HttpPut]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            try
            {
                Profile profile = db.Profiles.Find(id);

                if (TryUpdateModel(profile))
                {
                    profile.description = requestProfile.description;
                    profile.birthDate = Convert.ToDateTime(requestProfile.birthDate);
                    profile.hobbies = requestProfile.hobbies;
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost modificat!";
                    return RedirectToAction("Index");
                }
                return View(requestProfile);
            }
            catch (Exception e)
            {
                return View(requestProfile);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            TempData["message"] = "Profilul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}