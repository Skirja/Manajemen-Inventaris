using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Manajemen_Inventaris
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            // Handle root URL requests
            if (Request.Url.AbsolutePath == "/")
            {
                // Redirect root URL to Login.aspx
                string redirectUrl = VirtualPathUtility.ToAbsolute("~/Pages/Auth/Login.aspx");
                Response.Clear();
                Response.Redirect(redirectUrl);
            }
        }
    }
}