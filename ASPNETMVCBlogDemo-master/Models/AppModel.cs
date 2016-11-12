using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MarkdownDeep;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Reflection;
using System.IO;
using Ganss.XSS;

namespace MVCBlogDemo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Post> Posts { get; set; }
        public ApplicationUserInfo UserInfo { get; set; }
    }

    public class ApplicationUserInfo
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }
        public int AvatarId { get; set; }
        public Image Avatar { get; set; }
        public ICollection<Favourite> Favourites { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string DisplayName
        { // Returns Full name OR username if details have not been filled out.
            get
            {
                string fullName = FirstName + " " + LastName;
                if (String.IsNullOrWhiteSpace(fullName))
                {
                    return ApplicationUser.UserName;
                }
                return fullName;
            }
        }
    }

    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        private byte[] _content;

        // Return default image if none exists
        public byte[] Content
        {
            get
            {
                if (_content != null && _content.Length > 0) { return _content; }
                return System.IO.File.ReadAllBytes(MapPath("~/Content/Images/user-placeholder.jpg"));
            }
            set { _content = value; }
        }
        public ApplicationUserInfo ApplicationUserInfo { get; set; }

        // For using mappath inside seed method
        private string MapPath(string seedFile)
        {
            if (HttpContext.Current != null)
                return HostingEnvironment.MapPath(seedFile);

            var absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, ".." + seedFile.TrimStart('~').Replace('/', '\\'));

            return path;
        }
    }

    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        [StringLength(10000)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Favourite> Favourites { get; set; }
        public DateTime Date { get; set; }
        public string TimeFromNow { get { return MVCBlogDemo.App_Code.Utils.GetTimeSpan(Date); } } // Prettified relative time

        public ApplicationUser Author { get; set; }

        // Content is sanitized by https://github.com/mganss/HtmlSanitizer
        // iframes are currently enabled as the lib seems to do a good job of keeping them safe
        // I wouldn't trust this for production though.
        public string Render
        {
            get
            {
                var md = new MarkdownDeep.Markdown();
                string html = md.Transform(Content);
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                sanitizer.AllowedTags.Add("iframe");
                return sanitizer.Sanitize(html);
            }
        }

        // Only return a preview of the content
        public string RenderPreview
        {
            get
            {
                var md = new MarkdownDeep.Markdown();
                string content = Content;
                if (content.Length > 200)
                {
                    content = content.Substring(0, 200) + "...";
                }
                string html = md.Transform(content);
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                sanitizer.AllowedTags.Add("iframe");
                return sanitizer.Sanitize(html);
            }
        }

        // A rudimentary measure of how popular a post is
        public double Virality
        {
            get
            {
                if (Favourites != null)
                {
                    TimeSpan age = DateTime.UtcNow - Date;
                    return (double)Favourites.Count * Math.Exp(-Math.Log(2) * age.TotalDays);
                }
                else return 0;
            }
        }
    }

    public class Tag
    {
        public int Id { get; set; }
        public Post Post { get; set; }
        public string Name { get; set; }
    }

    public class Favourite
    {
        public int Id { get; set; }
        public Post Post { get; set; }
        public ApplicationUserInfo User { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Posts)
                .WithRequired(p => p.Author)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.UserInfo)
                .WithOptionalPrincipal(i => i.ApplicationUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUserInfo>()
                .HasOptional(i => i.Avatar)
                .WithOptionalPrincipal(a => a.ApplicationUserInfo)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUserInfo>()
                .HasMany(i => i.Favourites)
                .WithRequired(f => f.User)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithRequired(t => t.Post)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Favourites)
                .WithRequired(f => f.Post)
                .WillCascadeOnDelete(true);

            // Change the name of the table to be Users instead of AspNetUsers
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<ApplicationUserInfo> ApplicationUserInfoes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}