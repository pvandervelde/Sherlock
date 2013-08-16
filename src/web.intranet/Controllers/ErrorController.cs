//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Net;
using System.Web.Mvc;

namespace Sherlock.Web.Intranet.Controllers
{
    /// <summary>
    /// The controller that takes care of the error pages.
    /// </summary>
    public sealed class ErrorController : Controller
    {
        /// <summary>
        /// Provides the controller method for the error 404, page not found, URL.
        /// </summary>
        /// <returns>The 404 page.</returns>
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        /// <summary>
        /// Provides the controller method for the error page URL.
        /// </summary>
        /// <returns>The error page.</returns>
        public ActionResult ServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Todo: Pass the exception into the view model.
            // Also notify of internal error and stuff
            // Need Elmah for this maybe?
            {
                var exception = Server.GetLastError();
            }

            return View();
        }
    }
}
