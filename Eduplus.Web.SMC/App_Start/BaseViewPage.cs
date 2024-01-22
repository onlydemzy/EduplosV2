using Eduplos.Domain.CoreModule;
using Eduplos.Services.Implementations;
using KS.Web.Security;
using System.Web.Mvc;

namespace KS.UI.Web
{
    public abstract class BaseViewPage : WebViewPage
    {
        public virtual new CustomPrincipal User
        {
            get { return base.User as CustomPrincipal; }
        }
    }
    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new CustomPrincipal User
        {
            get { return base.User as CustomPrincipal; }
        }
        public UserData UserData
        {
            get
            {
                return System.Web.HttpContext.Current.Cache.Get("userData") as UserData;
                
            }
        }
    }
}