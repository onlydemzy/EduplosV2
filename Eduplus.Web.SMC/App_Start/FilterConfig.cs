using Eduplus.Web.SMC;
using System.Web;
using System.Web.Mvc;

namespace Eduplus.Web
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
