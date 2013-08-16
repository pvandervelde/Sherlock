//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sherlock.Shared.DataAccess;
using Sherlock.Web.Intranet.Models;

namespace Sherlock.Web.Intranet.Controllers
{
    /// <summary>
    /// The controller that takes care of the machine pages.
    /// </summary>
    public sealed class MachineController : Controller
    {
        private static MachineModel DescriptionToModel(MachineDescription m)
        {
            var physical = m as PhysicalMachineDescription;
            if (physical != null)
            {
                return new PhysicalMachineModel(physical);
            }

            var hyperv = m as HypervMachineDescription;
            if (hyperv != null)
            {
                return new HypervMachineModel(hyperv);
            }

            throw new UnknownMachineTypeException();
        }

        private static void AddOperatingSystems(IProvideEnvironmentContext context, dynamic viewBag)
        {
            var operatingSystems = context.OperatingSystems().Select(
                c => new SelectListItem
                {
                    Text = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} {1} - {2} - {3}",
                        c.Name,
                        c.ServicePack,
                        c.PointerSize,
                        c.CultureInfo),
                    Value = c.Id.ToString(CultureInfo.InvariantCulture),
                });
            viewBag.OperatingSystems = operatingSystems;
        }

        private static void AddHostMachines(IProvideEnvironmentContext context, dynamic viewBag)
        {
            var hostMachines = context.PhysicalMachines().Select(
                c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id,
                });
            viewBag.HostMachines = hostMachines;
        }

        private readonly IProvideEnvironmentContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineController"/> class.
        /// </summary>
        /// <param name="context">The machine context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public MachineController(IProvideEnvironmentContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// The controller method that returns the index page for the machine section.
        /// </summary>
        /// <returns>The index page.</returns>
        public ActionResult Index()
        {
            var environments = m_Context.Machines().Select(DescriptionToModel);
            return View(environments);
        }

        /// <summary>
        /// The controller method that returns a page containing the details of the machine
        /// with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The details page.</returns>
        public ActionResult Details(string id)
        {
            var machine = DescriptionToModel(m_Context.Machine(id));
            return View(machine);
        }

        /// <summary>
        /// The controller method that returns the page allowing the user to register a new machine.
        /// </summary>
        /// <returns>The machine create page.</returns>
        public ActionResult CreatePhysicalMachine()
        {
            AddOperatingSystems(m_Context, ViewBag);
            return View();
        }

        /// <summary>
        /// The controller method that is called upon posting a new machine.
        /// </summary>
        /// <param name="machine">The information about the new machine.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult CreatePhysicalMachine(PhysicalMachineModel machine)
        {
            if (ModelState.IsValid)
            {
                m_Context.Add(machine.ToDescription());
                m_Context.StoreChanges();

                return RedirectToAction("Index");
            }

            AddOperatingSystems(m_Context, ViewBag);
            return View(machine);
        }

        /// <summary>
        /// The controller method that returns the page allowing the user to register a new machine.
        /// </summary>
        /// <returns>The machine create page.</returns>
        public ActionResult CreateHypervMachine()
        {
            AddOperatingSystems(m_Context, ViewBag);
            AddHostMachines(m_Context, ViewBag);
            return View();
        }

        /// <summary>
        /// The controller method that is called upon posting a new machine.
        /// </summary>
        /// <param name="machine">The information about the new machine.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult CreateHypervMachine(HypervMachineModel machine)
        {
            if (ModelState.IsValid)
            {
                m_Context.Add(machine.ToDescription());
                m_Context.StoreChanges();

                return RedirectToAction("Index");
            }

            AddOperatingSystems(m_Context, ViewBag);
            AddHostMachines(m_Context, ViewBag);
            return View(machine);
        }

        /// <summary>
        /// The controller method that allows editing an existing machine.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The edit page.</returns>
        public ActionResult EditPhysicalMachine(string id)
        {
            var machine = DescriptionToModel(m_Context.Machine(id)) as PhysicalMachineModel;

            AddOperatingSystems(m_Context, ViewBag);
            return View(machine);
        }

        /// <summary>
        /// The controller method that is called upon posting an edited machine.
        /// </summary>
        /// <param name="machine">The machine.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult EditPhysicalMachine(PhysicalMachineModel machine)
        {
            if (ModelState.IsValid)
            {
                m_Context.Update(machine.ToDescription());
                m_Context.StoreChanges();
                return RedirectToAction("Index");
            }

            AddOperatingSystems(m_Context, ViewBag);
            return View(machine);
        }

        /// <summary>
        /// The controller method that allows editing an existing machine.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The edit page.</returns>
        public ActionResult EditHypervMachine(string id)
        {
            var machine = DescriptionToModel(m_Context.Machine(id)) as HypervMachineModel;

            AddOperatingSystems(m_Context, ViewBag);
            AddHostMachines(m_Context, ViewBag);
            return View(machine);
        }

        /// <summary>
        /// The controller method that is called upon posting an edited machine.
        /// </summary>
        /// <param name="machine">The machine.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        public ActionResult EditHypervMachine(HypervMachineModel machine)
        {
            if (ModelState.IsValid)
            {
                m_Context.Update(machine.ToDescription());
                m_Context.StoreChanges();
                return RedirectToAction("Index");
            }

            AddOperatingSystems(m_Context, ViewBag);
            AddHostMachines(m_Context, ViewBag);
            return View(machine);
        }

        /// <summary>
        /// The controller method that handles the deleting of a given machine.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The delete page.</returns>
        public ActionResult Delete(string id)
        {
            var machine = DescriptionToModel(m_Context.Machine(id));
            return View(machine);
        }

        /// <summary>
        /// The controller method that handles the posting of the deletion of a machine.
        /// </summary>
        /// <param name="id">The ID of the machine.</param>
        /// <returns>The post page.</returns>
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            m_Context.DeleteMachine(id);
            m_Context.StoreChanges();

            return RedirectToAction("Index");
        }
    }
}
