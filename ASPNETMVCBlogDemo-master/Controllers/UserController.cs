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
                .Include(info => info.ApplicationUser.Posts.Select(p => p.Favourites.Select(f => f.User.ApplicationUser)))
                .Include(info => info.Avatar)
                .Include(info => info.Favourites.Select(f => f.User))
                .Where(info => info.ApplicationUser.Id == id)
                .FirstOrDefault();

            return View(userInfo);
        }

        // GET: Favourites
        public ActionResult Favourites(string id)
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
            var posts = db.Posts
                .Include(p => p.Tags)
                .Include(p => p.Favourites.Select(f => f.User.ApplicationUser))
                .Include(p => p.Author)
                .Include(p => p.Author.UserInfo)
                .Include(p => p.Author.UserInfo.Avatar)
                .Where(p => p.Favourites.Any(f => f.User.ApplicationUser.Id == id));
            return View(posts);
        }
    }
}