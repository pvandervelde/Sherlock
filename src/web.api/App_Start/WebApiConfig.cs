﻿//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Http;

namespace Sherlock.Web.Api
{
    /// <summary>
    /// Handles the registration of the web configuration.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the web configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // config.Routes.MapHttpRoute(
            //     name: "DefaultApi",
            //     routeTemplate: "api/{controller}/{id}",
            //     defaults: new { id = RouteParameter.Optional });
            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            // config.EnableQuerySupport();
            //
            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
