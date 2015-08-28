using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCBlogDemo.Models
{
    public class UserInfoEditModel
    {
        public ApplicationUserInfo info { get; set; }
        public HttpPostedFileBase file { get; set; }
    }
}