using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCBlogDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlogDemo.Controllers
{
    public class UserInfoController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public UserInfoController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: UserInfo
        public ActionResult Index()
        {
            var info = getUserInfo();
            return View(info);
        }

        // GET
        public ActionResult Edit()
        {
            var info = getUserInfo();
            return View(info);
        }

        // POST
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id, FirstName, LastName, Bio, Avatar")] ApplicationUserInfo info)
        {
            if (ModelState.IsValid)
            {
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(info);
        }

        private ApplicationUserInfo getUserInfo()
        {
            string id = User.Identity.GetUserId();
            var info = db.ApplicationUserInfoes
                .Where(i => i.ApplicationUser.Id == id)
                .FirstOrDefault();
            return info;
        }
    }
}