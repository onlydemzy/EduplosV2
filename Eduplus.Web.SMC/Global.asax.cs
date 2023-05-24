using Eduplus.Domain.CoreModule;
using KS.Web.Security;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Eduplus.Web.SMC
{
    public class MvcApplication : System.Web.HttpApplication
    {

        static Cache _cache = null;
        protected void Application_Start()
        {
            _cache = Context.Cache;
            UserData udata = FetchUserData.CreateUserDataForCache();

            _cache.Insert("userData", udata, null,
                Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                CacheItemPriority.Default,
                new CacheItemRemovedCallback(RefreshUserDataCache));


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
            GlobalConfiguration.Configuration.Formatters.Add(new BinaryMediaFormatter());

        }
        //will remove in api endpoints
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
        }
        private void RefreshUserDataCache(string key,object item,CacheItemRemovedReason reason)
        {
            //UserData udata = FetchUserData.CreateUserDataForCache();
            _cache.Insert("userData", FetchUserData.CreateUserDataForCache(), new CacheDependency("uData"),
                Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                CacheItemPriority.Default,
                new CacheItemRemovedCallback(RefreshUserDataCache));

        }
        public override void Dispose()
        {
            base.Dispose();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies["KS_Eduplus_AKCOE"];
            if (authCookie != null)
            {

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
                var dUser = new CustomPrincipal(authTicket.Name);
                dUser.UserId = serializeModel.UserId;
                dUser.FullName = serializeModel.FullName;
                dUser.Username = serializeModel.Username;
                dUser.UserId = serializeModel.UserId;
                //CustomPrincipal user = (CustomPrincipal)Session["LoggedUser"];


                HttpContext.Current.User = dUser;
            }

        } /*
         protected void Application_PreSendRequestHeaders()
         {
             if(HttpContext.Current!=null)
             {
                 HttpContext.Current.Response.Headers.Remove("Server");
             }
         }
        
          * <system.webServer>
     <httpProtocol>
       <customHeaders>
         <add name ="X-Xss-Protection" value="1; mode=block"/>
         <add name="X-Content-Type-Options" value="nosniff"/>
         <remove name="X-Powered-By"/>
         <add name="Strict-Transport-Security" value="max-age=31536000"/>
       </customHeaders>
     </httpProtocol>
   </system.webServer>

          */
    }
}
