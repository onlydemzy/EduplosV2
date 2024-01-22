using System.Web.Http;

namespace Eduplos.Web.SMC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.EnableCors();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.MessageHandlers.Add(new ApiLogHandler());
            config.Formatters.Add(new BinaryMediaFormatter());
             
        }
    }
}
