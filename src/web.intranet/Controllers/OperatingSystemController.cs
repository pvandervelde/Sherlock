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
    /// The controller that takes care of the pages displaying information about 
    /// the registered operating systems.
    /// </summary>
    public sealed class OperatingSystemController : Controller
    {
        private static void AddArchitecturesAndCultures(dynamic viewBag)
        {
            viewBag.Architectures = OperatingSystemPointerSize.X86.ToSelectList();

            var cultures = OperatingSystemSupport.Cultures.Select(
                c => new SelectListItem
                {
                    Text = c.DisplayName,
                    Value = c.Name,
                });
            viewBag.Cultures = cultures;
        }

        private readonly IProvideEnvironmentContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingSystemController"/> class.
        /// </summary>
        /// <param name="context">The environment context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public OperatingSystemController(IProvideEnvironmentContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// The controller method that returns the index page for the operating systems section.
        /// </summary>
        /// <returns>The index page.</returns>
        public ActionResult Index()
        {
            var operatingSystems = m_Context.OperatingSystems().Select(o => new OperatingSystemModel(o));
            return View(operatingSystems);
        }

        /// <summary>
        /// The controller method that returns a page containing the details of the operating system
        /// with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the operating system.</param>
        /// <returns>The details page.</returns>
        public ActionResult Details(int id)
        {
            var operatingSystem = m_Context.OperatingSystem(id);
            return View(new OperatingSystemModel(operatingSystem));
        }

        /// <summary>
        /// The controller method that returns the page allowing the user to register a new operating
        /// system.
        /// </summary>
        /// <returns>The operating system create page.</returns>
        public ActionResult Create()
        {
            AddArchitecturesAndCultures(ViewBag);
            return View();
        }

        /// <summary>
        /// The controller method that is called upon posting a new operating system.
        /// </summary>
        /// <param name="operatingSystem">The information about the new operating system.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult Create(OperatingSystemModel operatingSystem)
        {
            if (ModelState.IsValid)
            {
                m_Context.Add(operatingSystem.ToDescription());
                m_Context.StoreChanges();

                return RedirectToAction("Index");
            }

            AddArchitecturesAndCultures(ViewBag);
            return View(operatingSystem);
        }

        /// <summary>
        /// The controller method that allows editing an existing operating system.
        /// </summary>
        /// <param name="id">The ID of the operating system.</param>
        /// <returns>The edit page.</returns>
        public ActionResult Edit(int id)
        {
            var operatingSystem = m_Context.OperatingSystem(id);
            AddArchitecturesAndCultures(ViewBag);
            return View(new OperatingSystemModel(operatingSystem));
        }

        /// <summary>
        /// The controller method that is called upon posting an edited operating system.
        /// </summary>
        /// <param name="operatingSystem">The operating system.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult Edit(OperatingSystemModel operatingSystem)
        {
            if (ModelState.IsValid)
            {
                m_Context.Update(operatingSystem.ToDescription());
                m_Context.StoreChanges();
                return RedirectToAction("Index");
            }

            AddArchitecturesAndCultures(ViewBag);
            return View(operatingSystem);
        }

        /// <summary>
        /// The controller method that handles the deleting of a given operating system.
        /// </summary>
        /// <param name="id">The ID of the operating system.</param>
        /// <returns>The delete page.</returns>
        public ActionResult Delete(int id)
        {
            var operatingSystem = m_Context.OperatingSystem(id);
            return View(new OperatingSystemModel(operatingSystem));
        }

        /// <summary>
        /// The controller method that handles the posting of the deletion of an operating system.
        /// </summary>
        /// <param name="id">The ID of the operating system.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            m_Context.DeleteOperatingSystem(id);
            m_Context.StoreChanges();

            return RedirectToAction("Index");
        }
    }
}
