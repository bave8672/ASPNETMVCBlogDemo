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
        public string TimeFromNow { get { return MVCBlogDemo.App_Code.Utils.GetTimeSpan(Date); } }

        public ApplicationUser Author { get; set; }

        public string Render
        {
            get
            {
                var md = new MarkdownDeep.Markdown();
                return md.Transform(Content);
            }
        }

        // This method does nothing and needs to be factored out.
        public string RenderPreview
        {
            get
            {
                string content = Render;

                // I wanted to return only part of the post however this can break the page if the post contains html

                //if (content.Length > 200)
                //{
                //    return content.Substring(0, 200) + "...";
                //}
                return content;
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
                .WillCascadeOnDelete();

            modelBuilder.Entity<ApplicationUser>()
                .HasRequired(u => u.UserInfo)
                .WithRequiredPrincipal(i => i.ApplicationUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUserInfo>()
                .HasOptional(i => i.Avatar)
                .WithOptionalPrincipal(a => a.ApplicationUserInfo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ApplicationUserInfo>()
                .HasMany(i => i.Favourites)
                .WithRequired(f => f.User)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithRequired(t => t.Post)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Favourites)
                .WithRequired(f => f.Post)
                .WillCascadeOnDelete();

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
    }
}