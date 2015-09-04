using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCBlogDemo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MVCBlogDemo.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public HomeController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Greeter()
        {
            var posts = db.Posts
                .Include(p => p.Tags)
                .Include(p => p.Favourites.Select(f => f.User.ApplicationUser))
                .Include(p => p.Author)
                .Include(p => p.Author.UserInfo)
                .Include(p => p.Author.UserInfo.Avatar)
                .ToList();
            var ordPosts = posts.
                OrderByDescending(p => p.Virality)
                .Take(3);
            return PartialView("_GreeterPartial", ordPosts);
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
    }
}