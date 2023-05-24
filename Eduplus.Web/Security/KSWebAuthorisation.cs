using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace KS.Web.Security
{
    public class KSWebAuthorisation : AuthorizeAttribute
    {
        
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //Create permission string based on the requested controller name and action name in the format 'controllername-action'
            var appUser = HttpContext.Current.User.Identity.Name;
            if(!string.IsNullOrEmpty(appUser))
            {
                string requiredPermission = String.Format("{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);


                ////////////Mine Trial Work?????????

                CustomPrincipal currentUser = (CustomPrincipal)HttpContext.Current.User;
                //var userRoles=(string[])HttpContext.Current.Session["roles"];
                //var userPermissions = (string[])HttpContext.Current.Session["permissions"];
                if (currentUser.Permissions.Count() > 0)
                {

                    string currentRequestUsername = filterContext.RequestContext.HttpContext.User.Identity.Name;
                    if (currentUser.Username == currentRequestUsername)//user confirmed, proceed if have required permissions
                    {
                        var chk = currentUser.HasPermission(requiredPermission);
                        if (!currentUser.HasPermission(requiredPermission) & !currentUser.IsSysAdmin)
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Unauthorised" }, { "controller", "Accounts" } });
                        }
                    }
                }
            }

            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Unauthorised" }, { "controller", "Accounts" } });
            }
            
            
        }
    }
}