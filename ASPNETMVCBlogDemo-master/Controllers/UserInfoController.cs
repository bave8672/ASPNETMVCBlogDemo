using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCBlogDemo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlogDemo.Controllers
{
    [Authorize]
    public class UserInfoController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public UserInfoController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: UserInfo
        public ActionResult Index()
        {
            var info = getUserInfo();
            return View(info);
        }

        // GET
        public ActionResult Edit(string customError = "")
        {
            var userInfo = getUserInfo();
            var model = new UserInfoEditModel { info = userInfo };
            ViewBag.customError = customError;
            return View(model);
        }

        // POST
        [HttpPost]
        public ActionResult Edit(UserInfoEditModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (model.file != null && model.file.ContentLength > 0)
                {
                    // Validate filesize
                    if (model.file.ContentLength > 6E5)
                    {
                        return RedirectToRoute(new { controller = "UserInfo", Action = "Edit", customError = "File is over 600kb" });
                    }

                    // Delete old image
                    var thisUser = db.ApplicationUserInfoes.AsNoTracking().Where(i => i.Id == model.info.Id).Include(i => i.Avatar).FirstOrDefault();
                    Image oldAvatar = new Image();
                    if (thisUser.Avatar != null)
                    {
                        oldAvatar = db.Images.Where(i => i.Id == thisUser.Avatar.Id).First();
                    }
                    if (oldAvatar.Content != null)
                    {
                        db.Images.Remove(oldAvatar);
                        db.SaveChanges();
                    }

                    // Read image data
                    Image Avatar = new Image();
                    Avatar.Size = model.file.ContentLength;
                    Avatar.Name = model.file.FileName;
                    Stream fileStream = model.file.InputStream;
                    byte[] imageBytes = new byte[fileStream.Length];
                    fileStream.Read(imageBytes, 0, imageBytes.Length);
                    Avatar.Content = imageBytes;

                    // Update database
                    db.Images.Add(Avatar);
                    model.info.Avatar = Avatar;
                }

                db.Entry(model.info).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        private ApplicationUserInfo getUserInfo()
        {
            string id = User.Identity.GetUserId();
            var info = db.ApplicationUserInfoes
                .Include(i => i.Avatar)
                .Where(i => i.ApplicationUser.Id == id)
                .FirstOrDefault();
            return info;
        }
    }
}