using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCBlogDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace MVCBlogDemo.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<MVCBlogDemo.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MVCBlogDemo.Models.ApplicationDbContext context)
        {
            var db = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Create Role Admin if it does not exist
            if (!RoleManager.RoleExists("Admin"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("Admin"));
            }

            //Create User=Admin with password=123456 if one does not exist
            if (!db.Users.Where(u => u.UserName == "Admin").Any())
            {
                var user = new ApplicationUser();
                user.UserName = "Admin";
                user.UserInfo = new ApplicationUserInfo { ApplicationUser = user };
                Image defaultAvatar = new Image() { Name = "defaultAvatar" };
                user.UserInfo.Avatar = defaultAvatar;
                var adminresult = UserManager.Create(user, "123456");

                //Add User Admin to Role Admin
                if (adminresult.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            // Try and save changes
            SaveChanges(db);
        }

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

        /// <summary>
        /// Wrapper for SaveChanges adding the Validation Messages to the generated exception
        /// </summary>
        /// <param name="context">The context.</param>
        private void SaveChanges(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }
    }
}
