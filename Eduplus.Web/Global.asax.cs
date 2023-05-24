using Castle.Windsor;
using Eduplus.Dependencies;
using Eduplus.Services.Contracts;
using KS.Web.Dependency;
using KS.Web.Security;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Eduplus.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private readonly IWindsorContainer _windsorContainer;
        //private readonly IGeneralDutiesService _generalServices;
        //static Cache deptsCache;
        public MvcApplication()
        {
            this._windsorContainer = new WindsorContainer()
             .Install(new ControllerDependencyConventions())
            .Install(new ServicesDependency())
            .Install(new DataDependency());
           // _generalServices = generalServices;
        }
        public override void Dispose()
        {
            this._windsorContainer.Dispose();
            base.Dispose();
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var controllerFactory = new WindsorControllerFactory(_windsorContainer.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
           
        }

        /*void RepopulateDepartments(string key, object item, CacheItemRemovedReason reason)
        {
            var depts = _generalServices.FetchDepartments();
            deptsCache = Context.Cache;
            deptsCache.Insert("departments", depts, null, DateTime.Now.AddMinutes(15),
                Cache.NoSlidingExpiration,CacheItemPriority.Default,
                new CacheItemRemovedCallback(RepopulateDepartments));
        }*/
        
    }
}
