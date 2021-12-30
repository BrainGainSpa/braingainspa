using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace braingainspa
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            "Default", // Route name
            "{controller}/{action}/{id}",
            new { controller = "Home", action = "Default", id = UrlParameter.Optional },
            new[] { "braingainspa.Controllers" });

            routes.MapRoute(
           "FreeBees", // Route name
           "{controller}/{action}/{id}",
           new { controller = "Quiz", action = "FreeBees", id = UrlParameter.Optional },
           new[] { "braingainspa.Controllers" });

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Default", id = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "myIndex", id = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Quiz", action = "AdminDashboard", id = UrlParameter.Optional }
            //);
        }
    }
}
