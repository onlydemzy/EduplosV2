﻿using System.Web;
using System.Web.Optimization;

namespace Eduplos.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
           
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.7.1.js",
                        "~/Scripts/jquery.easing.1.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                "~/Scripts/moment.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/sweetalert").Include(
                "~/Scripts/sweetalert.min.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

           
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.bundle.min.js",
                     "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-3.4.2.js", 
                "~/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                "~/Scripts/chart.js"));

            

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                     "~/Content/font-awesome.css",
                     "~/Content/sb-admin-2.min.css",
                      "~/Content/site.css",
                      "~/Styles/sweetalert.css"));
        }
    }
}
