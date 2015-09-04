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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MVCBlogDemo.Models;
using Owin;

namespace MVCBlogDemo.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db;
        public UserManager<ApplicationUser> UserManager { get; private set; }
                private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationUsersController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public ApplicationUsersController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            db = new ApplicationDbContext();
        }

        // GET: ApplicationUsers
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Where(u => u.Id == id).First();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserName,Email,PasswordHash,SecurityStamp,IsConfirmed")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Where(u => u.Id == id).First();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,Email,PasswordHash,SecurityStamp,IsConfirmed")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(applicationUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Where(u => u.Id == id).First();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, bool? notAdmin)
        {
            ApplicationUser applicationUser = db.Users.Where(u => u.Id == id).First();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            db.Users.Remove(applicationUser);
            var favourites = await db.Favourites
                .Where(f => f.User.ApplicationUser.Id == id)
                .ToListAsync();
            foreach (Favourite fav in favourites)
            {
                db.Favourites.Remove(fav);
            }
            await db.SaveChangesAsync();
            if (notAdmin != null)
            {
                AuthenticationManager.SignOut();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
