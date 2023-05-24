using KS.Web.Security;
using System.Web.Mvc;

namespace Eduplus.Web.Controllers
{
    public class BaseController : Controller
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }


    }
}