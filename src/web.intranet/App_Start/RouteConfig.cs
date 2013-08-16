//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Mvc;
using System.Web.Routing;

namespace Sherlock.Web.Intranet
{
    /// <summary>
    /// Handles the registration of the routes.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers the global routes.
        /// </summary>
        /// <param name="routes">The route collection.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                name: "Error - 404",
                url: "NotFound",
                defaults: new { controller = "Error", action = "NotFound" });

            routes.MapRoute(
                name: "Error - 500",
                url: "ServerError",
                defaults: new { controller = "Error", action = "ServerError" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
