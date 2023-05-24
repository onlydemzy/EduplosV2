using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.Security;

namespace WebApi
{
    
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Add this line to enable detail mode in release
            
            //app.UseHttpsRedirection();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
