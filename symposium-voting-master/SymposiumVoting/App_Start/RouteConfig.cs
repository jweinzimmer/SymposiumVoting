using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SymposiumVoting
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes(); //Attribute Routing
            routes.MapRoute(
                "Project Vote",                                           // Route name
                "Vote/{id}",                            // URL with parameters
                new { controller = "Projects", action = "project_vote" }
                );// Parameter defaults
           routes.MapRoute(
                "Reports",                                           // Route name
                "Reports",                            // URL with parameters
                new { controller = "Symposiums", action = "report_index" }
                );// Parameter defaults
            //routes.MapRoute(
            //    "Symposiums Index",                                           // Route name
            //    "Symposiums",                            // URL with parameters
            //    new { controller = "Symposiums", action = "Index" }
            //    );// Parameter defaults
            //routes.MapRoute(
            //    "Symposium Details",                                           // Route name
            //    "Symposiums/{id}",                            // URL with parameters
            //    new { controller = "Symposiums", action = "Details" }
            //    );// Parameter defaults

            routes.MapRoute(
                "Account",                                           // Route name
                "admin-unf-login",                            // URL with parameters
                new { controller = "Account", action = "Login" }
                );// Parameter defaults
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Projects", action = "desktop_vote", id = UrlParameter.Optional }
            );
        }

    }
}
