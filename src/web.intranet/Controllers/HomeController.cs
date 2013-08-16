//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Nuclei;
using Sherlock.Shared.DataAccess;
using Sherlock.Web.Intranet.Models;
using Sherlock.Web.Intranet.Properties;

namespace Sherlock.Web.Intranet.Controllers
{
    /// <summary>
    /// The controller that takes care of the landing pages.
    /// </summary>
    public sealed class HomeController : Controller
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

        private static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            Debug.Assert(attribute.Length == 1, "There should be a copyright attribute.");

            return (attribute[0] as AssemblyFileVersionAttribute).Version;
        }

        private static string GetCopyright()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            Debug.Assert(attribute.Length == 1, "There should be a copyright attribute.");

            return (attribute[0] as AssemblyCopyrightAttribute).Copyright;
        }

        private static string GetLibraryLicenses()
        {
            var licenseXml = EmbeddedResourceExtracter.LoadEmbeddedStream(
                Assembly.GetExecutingAssembly(),
                @"Sherlock.Console.Properties.licenses.xml");
            var doc = XDocument.Load(licenseXml);
            var licenses = from element in doc.Descendants("package")
                           select new
                           {
                               Id = element.Element("id").Value,
                               Version = element.Element("version").Value,
                               Source = (element.Element("url").FirstNode as XCData).Value,
                               License = (element.Element("licenseurl").FirstNode as XCData).Value,
                           };

            var builder = new StringBuilder();
            builder.AppendLine(Resources.About_OtherPackages_Intro);
            foreach (var license in licenses)
            {
                builder.AppendLine(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.About_OtherPackages_IdAndLicense,
                        license.Id,
                        license.Version,
                        license.Source));
            }

            return builder.ToString();
        }

        private readonly IProvideEnvironmentContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="context">The machine context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public HomeController(IProvideEnvironmentContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// Gets the index, or landing page, for the machine.
        /// </summary>
        /// <returns>The index page.</returns>
        public ActionResult Index()
        {
            var environments = m_Context.Machines().Select(DescriptionToModel);
            return View(environments);
        }

        /// <summary>
        /// Gets the about page for the machine.
        /// </summary>
        /// <returns>The about page.</returns>
        public ActionResult About()
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.About_ApplicationAndVersion,
                    GetVersion()));

            builder.AppendLine(GetCopyright());
            builder.AppendLine(GetLibraryLicenses());

            ViewBag.Message = builder.ToString();

            return View();
        }
    }
}
