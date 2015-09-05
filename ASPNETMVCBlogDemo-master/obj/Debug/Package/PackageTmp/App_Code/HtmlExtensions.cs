using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCBlogDemo.App_Code
{
    // Extension for displaying images, adapted from http://stackoverflow.com/a/24082406
    // Usage: @Html.Image(Model.MyImageBytes, new { attr = "AttributeName" })
    public static class HtmlExtensions
    {
        public static MvcHtmlString Image(this HtmlHelper html, byte[] image, object attributes)
        {
            var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
            string attrStr = "";
            if (attributes != null)
            {
                foreach (var prop in attributes.GetType().GetProperties())
                {
                    attrStr += String.Format("{0}='{1}' ", prop.Name, prop.GetValue(attributes, null));
                }
            }
            string htmlString = String.Format("<img src='{0}' {1} />", img, attrStr);
            return new MvcHtmlString(htmlString);
        }
    }
}