using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ACG.EA.AppCentre.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			config.MapHttpAttributeRoutes();
            
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute("Default", "", new { controller = "Home" });
            config.Routes.MapHttpRoute("AppcentreApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }
            );
        }
    }
}
