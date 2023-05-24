using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Eduplus.Web.Startup))]
namespace Eduplus.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
