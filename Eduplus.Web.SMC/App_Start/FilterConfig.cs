using Eduplos.Web.SMC;
using System.Web;
using System.Web.Mvc;

namespace Eduplos.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RemoveUnwantedHeaderAttribute());
            //filters.Add(new KS.Web.Security.KSHttpsAttribute());

        }
    }
}
