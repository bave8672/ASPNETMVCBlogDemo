using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MarkdownDeep;

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

        public string Bio { get; set; }
        public byte[] Avatar { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string DisplayName { // Returns Full name OR username if details have not been filled out.
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

    public class Post
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(10000)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public ApplicationUser Author { get; set; }

        public string Render
        {
            get
            {
                var md = new MarkdownDeep.Markdown();
                return md.Transform(Content);
            }
        }

        public string RenderPreview
        {
            get
            {
                string content = Render;
                if (content.Length > 200)
                {
                    return content.Substring(0, 200) + "...";
                }
                return content;
            }
        }
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
                .HasRequired(u => u.UserInfo)
                .WithRequiredPrincipal(i => i.ApplicationUser);

            modelBuilder.Entity<ApplicationUserInfo>()
                .HasRequired(i => i.ApplicationUser)
                .WithRequiredDependent(u => u.UserInfo);

            // Change the name of the table to be Users instead of AspNetUsers
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<ApplicationUserInfo> ApplicationUserInfoes { get; set; }
    }


}