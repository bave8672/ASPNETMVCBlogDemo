using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCBlogDemo.Startup))]
namespace MVCBlogDemo
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
