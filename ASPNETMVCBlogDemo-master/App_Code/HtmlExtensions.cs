using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlogDemo.App_Code
{
    // Extension for displaying images, from http://stackoverflow.com/a/24082406
    // uSAGE: @Html.Image(Model.MyImageBytes)
    public static class HtmlExtensions
    {
        public static MvcHtmlString Image(this HtmlHelper html, byte[] image)
        {
            var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
            return new MvcHtmlString("<img src='" + img + "' />");
        }
    }
}