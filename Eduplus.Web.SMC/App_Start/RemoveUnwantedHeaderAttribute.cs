using System.Web.Mvc;

namespace Eduplus.Web.SMC
{
    public class RemoveUnwantedHeaderAttribute:ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Remove("Server");
            base.OnResultExecuted(filterContext);
        }
    }
}