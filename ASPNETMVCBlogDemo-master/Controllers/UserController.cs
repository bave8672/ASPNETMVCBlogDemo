using Microsoft.AspNet.Identity;
using MVCBlogDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MVCBlogDemo.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public UserController()
        {
            db = new ApplicationDbContext();
        }

        // GET: User info with related info and posts
        public ActionResult Profile(string id)
        {
            ViewBag.isCurrentUser = false;
            if (String.IsNullOrEmpty(id))
            {
                id = User.Identity.GetUserId();
            }
            if (id == User.Identity.GetUserId())
            {
                ViewBag.isCurrentUser = true;
            }
            var userInfo = db.ApplicationUserInfoes
                .Include(info => info.ApplicationUser)
                .Include(info => info.ApplicationUser.Posts)
                .Include(info => info.Avatar)
                .Where(info => info.ApplicationUser.Id == id)
                .FirstOrDefault();

            return View(userInfo);
        }
    }
}