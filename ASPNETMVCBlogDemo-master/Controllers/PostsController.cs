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
using System.Text.RegularExpressions;

namespace MVCBlogDemo.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public PostsController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Posts
        public async Task<ActionResult> Index()
        {
            return View(await db.Posts.ToListAsync());
        }

        // Custom view
        // GET: Posts/View/5
        [AllowAnonymous]
        public async Task<ActionResult> View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = await db.Posts
                .Include(p => p.Author)
                .Include(p => p.Author.UserInfo)
                .Include(p => p.Author.UserInfo.Avatar)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null || post.Author == null) // just in case post/ author no longer exists
            {
                return HttpNotFound();
            }
            return View(post);
        }
        // GET: Posts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = await db.Posts.FindAsync(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content")] Post post)
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                post.Author = currentUser;
                post.Date = System.DateTime.UtcNow;
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return RedirectToAction("View", new { id = post.Id });
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = await db.Posts.FindAsync(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,Date")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = await db.Posts.FindAsync(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Post post = await db.Posts.FindAsync(id);
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Partial view for the front page
        [AllowAnonymous]
        public ActionResult RenderPartial(int offset)
        {
            var posts = getNewPosts(offset);
            return PartialView("_RenderPartial", posts);
        }

        [AllowAnonymous]
        private List<Post> getNewPosts(int ofs)
        { 
            var posts = db.Posts
                .Include(post => post.Author)
                .Include(post => post.Author.UserInfo)
                .Include(post => post.Author.UserInfo.Avatar)
                .OrderByDescending(post => post.Date)
                .Skip(ofs)
                .Take(5) // Take 5 posts each time
                .ToList();
            return posts;
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
