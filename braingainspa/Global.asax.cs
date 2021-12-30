using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace braingainspa
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                //log the error
                Response.Redirect("/");
                //Response.Redirect("/Views/Shared/Error.cshtml");


            }

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //Not every request is called
            //Code that runs when the application shuts down and executes after the last HttpApplication is destroyed
            //For example, IIS restart, file updates, process recycling lead to application conversion to another application domain
            Session.Abandon();
            
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Not every request is called
            //Execution at the end or expiration of a session
            //This method will be invoked regardless of the explicit empty Session or automatic expiration of  Session timeouts in the code
            Session.Abandon();
        }
    }
}
