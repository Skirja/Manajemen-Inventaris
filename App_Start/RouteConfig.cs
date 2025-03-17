using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Manajemen_Inventaris
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Add a route for the root URL to redirect to Login.aspx
            routes.MapPageRoute(
                "Default",
                "",
                "~/Pages/Auth/Login.aspx"
            );

            routes.MapRoute(
                name: "DefaultMVC",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
