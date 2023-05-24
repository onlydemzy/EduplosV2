using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi
{
    public class IPFiltering:ActionFilterAttribute
    {
       
        public override void OnActionExecuting(HttpActionContext context)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;

            if (!IsIpAddressAllowed(ipAddress.Trim()))
            {
                context.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }

            base.OnActionExecuting(context);
        }

        private bool IsIpAddressAllowed(string IpAddress)
        {
            if (!string.IsNullOrWhiteSpace(IpAddress))
            {
                string[] addresses = Convert.ToString(ConfigurationManager.AppSettings["AllowedIPAddresses"]).Split(',');
                return addresses.Where(a => a.Trim().Equals(IpAddress, StringComparison.InvariantCultureIgnoreCase)).Any();
            }
            return false;
        }
    }
}