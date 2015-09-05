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
    public class SearchController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public SearchController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Search
        public ActionResult Index(string q = "", int ofs = 0)
        {
            // Data for pagination
            ViewBag.q = q;
            ViewBag.ofs = ofs;
            var results = searchPosts(q, ofs, false);
            if (ofs == 0) { ViewBag.prevClass = "disabled"; } else { ViewBag.prevClass = ""; }
            if (results.Count < 5) { ViewBag.nextClass = "disabled"; } else { ViewBag.nextClass = ""; }
            return View(results);
        }

        // GET: 3 Relevant posts
        public ActionResult _RelevantPostsPartial(Post post)
        {
            IEnumerable<Post> results;
            if (post.Tags != null && post.Tags.Any())
            {
                string q = "";
                foreach (Tag tag in post.Tags)
                {
                    q += tag.Name + ",";
                }
                results = searchPosts(q, 0, true);
            }
            else
            {
                results = searchPosts(post.Title, 0, true);
            }
            return PartialView("_RelevantPostsPartial", results.Where(p => p.Id != post.Id).Take(3));
        }

        // Class for the search method below
        private class SearchResult
        {
            public Post Post { get; set; }
            public int Relevance { get; set; }
        }

        private List<Post> searchPosts(string query, int ofs, bool? keepAll)
        {
            // Get all posts
            var allPosts = db.Posts
                .Include(p => p.Tags)
                .Include(p => p.Favourites.Select(f => f.User.ApplicationUser))
                .Include(p => p.Author)
                .Include(p => p.Author.UserInfo)
                .Include(p => p.Author.UserInfo.Avatar)
                .ToList();

            var searchResults = new List<SearchResult>();
            var results = new List<Post>();

            // Special case q = "", return pseudorandom results
            if (String.IsNullOrWhiteSpace(query))
            {
                results = allPosts.Skip(ofs).Take(5).ToList();
            }

            // Default custom search
            else
            {
                // Split query into words
                string[] searchTerms = Regex.Split(query, @"[^\w]+");

                // Weight posts according to how many matches they get with the search terms
                foreach (Post post in allPosts)
                {
                    SearchResult searchResult = new SearchResult { Post = post, Relevance = 0 };
                    foreach (string term in searchTerms)
                    {
                        string t = "(?i)" + term + "(?-i)"; // Ignores capitalisation

                        // Match query in title and content
                        searchResult.Relevance += Regex.Matches(post.Title, t).Count;
                        searchResult.Relevance += Regex.Matches(post.Content, t).Count;

                        // Match query in tags (and weight them more heavily)
                        foreach (Tag tag in post.Tags)
                        {
                            if (Regex.IsMatch(tag.Name, t))
                            {
                                searchResult.Relevance += 10;
                            }
                        }
                    }

                    // Discard irrelevant matches (unless keepAll is set to true)
                    if (keepAll == true || searchResult.Relevance > 0)
                    {
                        searchResults.Add(searchResult);
                    }
                }

                // Take the most relevant
                searchResults = searchResults.OrderByDescending(r => r.Relevance).ToList();
                for (int i = ofs; i < Math.Min(ofs + 5, searchResults.Count); i++)
                {
                    results.Add(searchResults[i].Post);
                }
            }

            return results;
        }
    }
}