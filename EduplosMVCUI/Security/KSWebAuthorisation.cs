using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Web.Security;

namespace KS.Web.Security
{
    public class KSWebAuthorisation : AuthorizeAttribute
    {

        private readonly ValidateAntiForgeryTokenAttribute _validateAntiForgeryTokenAttribute;
        private const string FieldName = "__RequestVerificationToken";
        public KSWebAuthorisation()
        {
            _validateAntiForgeryTokenAttribute = new ValidateAntiForgeryTokenAttribute();
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //Create permission string based on the requested controller name and action name in the format 'controllername-action'
            //confirm if session has expired
            if (HttpContext.Current.Session["LoggedUser"] == null)//redirect to login page
            {

                             

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Accounts",
                            action = "Login",
                            area = "",
                            returnUrl = filterContext.HttpContext.Request.Url?.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                        }
                        )
                    );


            }
            // ValidateAntifogeryAttribute(filterContext);

            if(!HttpContext.Current.Request.IsAuthenticated)
            {
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Accounts",
                            action = "Login",
                            area = "",
                            returnUrl = filterContext.HttpContext.Request.Url?.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                        }
                        )
                    );
            }

            else
            {
                var appUser = HttpContext.Current.User.Identity.Name;
                string requiredPermission = String.Format("{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
                string[] anonymousPermissions = AnonymousPermissions();

                ////////////Mine Trial Work?????????

                CustomPrincipal currentUser = (CustomPrincipal)HttpContext.Current.User;
                var user = (CustomPrincipal)HttpContext.Current.Session["LoggedUser"];

                if (user != null)

                {
                    currentUser = user;
                    string currentRequestUsername = filterContext.RequestContext.HttpContext.User.Identity.Name;
                    if (currentUser.Username == currentRequestUsername)//user confirmed, proceed if have required permissions
                    {
                        var chk = currentUser.HasPermission(requiredPermission);
                        if (!currentUser.HasPermission(requiredPermission) & !currentUser.IsSysAdmin & !anonymousPermissions.Contains(requiredPermission))
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Unauthorised" }, { "controller", "Accounts" } });
                        }
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Unauthorised" }, { "controller", "Accounts" } });
                    }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Accounts",
                            action = "Login",
                            area = "",
                            returnUrl = filterContext.HttpContext.Request.Url?.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                        }
                        )
                    );
                }
            }




        }

        private string[] AnonymousPermissions()
        {
            string[] perms = { "Student-CheckIfQualifiedToRegister", "Student-CheckIfAlreadyRegistered", "Student-PopulateCourse" };
            return perms;

        }
        private void ValidateAntifogeryAttribute(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.ContentType.ToLower().Contains("application/json"))
            {
                var bytes = new byte[filterContext.HttpContext.Request.InputStream.Length];
                filterContext.HttpContext.Request.InputStream.Read(bytes, 0, bytes.Length);
                filterContext.HttpContext.Request.InputStream.Position = 0;
                var json = Encoding.UTF8.GetString(bytes);
                var jsonObject = JObject.Parse(json);
                var value = (string)jsonObject[FieldName];
                var httpCookie = filterContext.HttpContext.Request.Cookies[AntiForgeryConfig.CookieName];
                if (httpCookie != null)
                {
                    AntiForgery.Validate(httpCookie.Value, value);
                }
                else
                {
                    throw new HttpAntiForgeryException("Anti forgery token cookie not found");
                }
            }
            else
            {
                _validateAntiForgeryTokenAttribute.OnAuthorization(filterContext);
            }
        }
    }
}