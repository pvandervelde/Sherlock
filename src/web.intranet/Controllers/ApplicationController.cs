//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Mvc;
using Sherlock.Shared.DataAccess;
using Sherlock.Web.Intranet.Models;

namespace Sherlock.Web.Intranet.Controllers
{
    /// <summary>
    /// The controller that takes care of the application pages.
    /// </summary>
    public sealed class ApplicationController : Controller
    {
        private readonly IProvideEnvironmentContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="context">The machine context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ApplicationController(IProvideEnvironmentContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// The controller method that returns the index page for the application section.
        /// </summary>
        /// <returns>The index page.</returns>
        public ActionResult Index()
        {
            var applications = m_Context.Applications().Select(a => new ApplicationModel(a));
            return View(applications);
        }

        /// <summary>
        /// The controller method that returns a page containing the details of the application
        /// with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <returns>The details page.</returns>
        public ActionResult Details(int id)
        {
            var application = m_Context.Application(id);
            return View(new ApplicationModel(application));
        }

        /// <summary>
        /// The controller method that returns the page allowing the user to register a new application.
        /// </summary>
        /// <returns>The application create page.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// The controller method that is called upon posting a new application.
        /// </summary>
        /// <param name="application">The information about the new application.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult Create(ApplicationModel application)
        {
            if (ModelState.IsValid)
            {
                m_Context.Add(application.ToDescription());
                m_Context.StoreChanges();

                return RedirectToAction("Index");
            }

            return View(application);
        }

        /// <summary>
        /// The controller method that allows editing an existing application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <returns>The edit page.</returns>
        public ActionResult Edit(int id)
        {
            var application = m_Context.Application(id);
            return View(new ApplicationModel(application));
        }

        /// <summary>
        /// The controller method that is called upon posting an edited application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult Edit(ApplicationModel application)
        {
            if (ModelState.IsValid)
            {
                m_Context.Update(application.ToDescription());
                m_Context.StoreChanges();
                return RedirectToAction("Index");
            }

            return View(application);
        }

        /// <summary>
        /// The controller method that handles the deleting of a given application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <returns>The delete page.</returns>
        public ActionResult Delete(int id)
        {
            var application = m_Context.Application(id);
            return View(new ApplicationModel(application));
        }

        /// <summary>
        /// The controller method that handles the posting of the deletion of an application.
        /// </summary>
        /// <param name="id">The ID of the application.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            m_Context.DeleteApplication(id);
            m_Context.StoreChanges();

            return RedirectToAction("Index");
        }
    }
}
