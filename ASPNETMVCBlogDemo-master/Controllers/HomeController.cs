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

        public ActionResult Profile()
        {
            // Get the current logged in User and look up the user in ASP.NET Identity
            string userId = User.Identity.GetUserId().ToString();
            var user = db.ApplicationUserInfoes
                .Include(info => info.ApplicationUser)
                .Include(info => info.ApplicationUser.Posts)
                .Where(info => info.ApplicationUser.Id == userId)
                .First();
            return View(user);
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