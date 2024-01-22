using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

namespace Eduplos.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {

                LoginPath = new PathString("/Accounts/Login"),
                LogoutPath = new PathString("/Accounts/LogOut"),
                ReturnUrlParameter = "/",
                ExpireTimeSpan = TimeSpan.FromMinutes(180.0),
                AuthenticationType="ApplicationCookie",
                SlidingExpiration = true,
                 
            });            
           
        }
    }
}