using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Http;
using System.Web.Routing;

namespace ErrorLogger
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //{
        //    filters.Add(new HandleErrorAttribute(), 2); //by default added


        //    filters.Add(new HandleErrorAttribute
        //    {
        //        View = "Error"
        //    }, 1);
        //}

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
    }
}
