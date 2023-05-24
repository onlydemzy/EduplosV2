using EduplosMVCUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EduplosMVCUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult _menuPartial()
        {
            MenuItem mi = new MenuItem();
            var ms = mi.GetMenuItems();
            return PartialView(mi.GetMenuItems());
        }
    }
}