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
        [Authorize(Roles="Admin")]
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
                .Include(p => p.Tags)
                .Include(p => p.Favourites.Select(f => f.User.ApplicationUser))
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
        [Authorize(Roles="Admin")]
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Content,Tags")] Post post, string tagString)
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId());
            post.Author = currentUser;

            if (ModelState.IsValid)
            {
                post.Tags = new List<Tag>();
                AddTags(post, tagString);
                post.Title = !String.IsNullOrEmpty(post.Title) ? post.Title : "";
                post.Content = !String.IsNullOrEmpty(post.Content) ? post.Content : "";
                post.Favourites = new List<Favourite>();
                post.Date = System.DateTime.UtcNow;
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return RedirectToAction("View", new { id = post.Id });
            }

            return View(post);
        }

        // Recreates and adds tags to post from posted string
        private void AddTags(Post post, string tagString)
        {
            if (!String.IsNullOrEmpty(tagString))
            {
                string[] _tags = Regex.Split(tagString, @"[^\w]+");
                var uniqTags = new List<string>();
                foreach (Tag tag in post.Tags)
                {
                    post.Tags.Remove(tag);
                }
                foreach (string tag in _tags)
                {
                    if (!uniqTags.Contains(tag))
                    {
                        uniqTags.Add(tag);
                        post.Tags.Add(new Tag { Name = tag, Post = post });
                    }
                }
            }
        }

        // GET: Posts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = await db.Posts
                .Include(p => p.Tags)
                .Include(p => p.Author)
                .Where(p => p.Id == id)
                .FirstAsync();
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,Tags")] Post post, string tagString)
        {
            if (ModelState.IsValid)
            {
                AddTags(post, tagString);
                post.Title = !String.IsNullOrEmpty(post.Title) ? post.Title : "";
                post.Content = !String.IsNullOrEmpty(post.Content) ? post.Content : "";
                db.Entry(post).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("View", new { id = post.Id });
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public void Test(int? id)
        {
            if (id == null)
            {
                return;
            }
            Post post = db.Posts
                .Include(p => p.Author)
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (post == null)
            {
                return;
            }
            if (post.Author.Id != User.Identity.GetUserId())
            {
                return;
            }
            db.Posts.Remove(post);
            db.SaveChanges();
            return;
        }

        [HttpPost]
        public void DeletePost(int? id)
        {
            if (id == null)
            {
                return;
            }
            Post post = db.Posts
                .Include(p => p.Author)
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (post == null)
            {
                return;
            }
            if (post.Author.Id != User.Identity.GetUserId())
            {
                return;
            }
            db.Posts.Remove(post);
            db.SaveChanges();
            return;
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
                .Include(post => post.Favourites.Select(f => f.User.ApplicationUser))
                .Include(post => post.Author)
                .Include(post => post.Author.UserInfo)
                .Include(post => post.Author.UserInfo.Avatar)
                .OrderByDescending(post => post.Date)
                .Skip(ofs)
                .Take(5) // Take 5 posts each time
                .ToList();
            return posts;
        }

        [HttpPost]
        public void HandleFavourite(int postId, bool favouriting)
        {
            Post post = db.Posts
                .Include(p => p.Favourites.Select(f => f.User.ApplicationUser))
                .Where(p => p.Id == postId).First();
            string userId = User.Identity.GetUserId();
            var user = db.Users
                .Include(u => u.UserInfo)
                .Where(u => u.Id == userId).First();

            // Favouriting
            if (favouriting)
            {
                // Just in case post somehow has no favourites
                if (post.Favourites == null)
                {
                    post.Favourites = new List<Favourite>();
                }
                Favourite newFavourite = new Favourite { Post = post, User = user.UserInfo };
                post.Favourites.Add(newFavourite);
            }

            // Unfavouriting
            else if (post.Favourites != null && post.Favourites.Where(f => f.User == user.UserInfo).First() != null)
            {
                Favourite favourite = post.Favourites.Where(f => f.User == user.UserInfo).First();
                db.Favourites.Remove(favourite);
            }

            db.SaveChanges();
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
