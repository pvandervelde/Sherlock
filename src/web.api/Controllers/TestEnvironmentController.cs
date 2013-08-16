//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Xml.Linq;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api.Controllers
{
    /// <summary>
    /// The controller that takes care of the test environment API calls.
    /// </summary>
    public sealed class TestEnvironmentController : ApiController
    {
        private static OperatingSystemDescription ExtractOperatingSystem(XDocument xmlContent)
        {
            // Expecting the following
            // <operatingsystem name="{OPERATING_SYSTEM_NAME}" 
            //                  servicepack="{SERVICE_PACK}"
            //                  culture="{CULTURE}"
            //                  architecturepointersize="{POINTER_SIZE}" />
            var osElement = xmlContent.Descendants("operatingsystem").First();
            var name = osElement.Attribute("name").Value;
            var servicePack = osElement.Attribute("servicepack").Value;
            var culture = osElement.Attribute("culture").Value;
            var pointerSize = int.Parse(osElement.Attribute("architecturepointersize").Value, CultureInfo.InvariantCulture);

            return new OperatingSystemDescription
            {
                Name = name,
                ServicePack = servicePack,
                CultureInfo = (!string.IsNullOrWhiteSpace(culture)) ? new CultureInfo(culture) : CultureInfo.InvariantCulture,
                PointerSize = (OperatingSystemPointerSize)Enum.ToObject(typeof(OperatingSystemPointerSize), pointerSize),
            };
        }

        private readonly IProvideTestingContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironmentController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public TestEnvironmentController(IProvideTestingContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// Registers a new test environment.
        /// </summary>
        /// <param name="name">The name of the test environment.</param>
        /// <param name="test">The ID of the test to which the environment belongs.</param>
        /// <returns>An XML document containing the ID of the new test.</returns>
        [HttpPost]
        public int Register(string name, int test)
        {
            var content = Request.Content;
            var textContent = content.ReadAsStringAsync().Result;
            var xmlContent = XDocument.Parse(textContent);

            var operatingSystem = ExtractOperatingSystem(xmlContent);
            var operatingSystemId = m_Context.OperatingSystems()
                .Where(
                    o => string.Equals(o.Name, operatingSystem.Name, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(o.ServicePack, operatingSystem.ServicePack, StringComparison.OrdinalIgnoreCase)
                        && o.CultureInfo.Equals(operatingSystem.CultureInfo)
                        && (o.PointerSize == operatingSystem.PointerSize))
                .Select(o => o.Id)
                .First();

            var environment = new TestEnvironment
                {
                    Name = name,
                    DesiredOperatingSystemId = operatingSystemId,
                    TestId = test,
                };

            m_Context.Add(environment);
            m_Context.StoreChanges();

            SetApplicationsForEnvironment(xmlContent, environment.Id);

            return environment.Id;
        }

        private void SetApplicationsForEnvironment(XDocument xmlContent, int environmentId)
        {
            // Expecting the following
            // <applications>
            //     <application name="{APPLICATION_NAME}"
            //                  version="{APPLICATION_VERSION}" />
            //     
            //      .... more applications here ...
            //     
            // </applications>
            var applicationsNode = xmlContent.Descendants("applications").First();
            foreach (var applicationNode in applicationsNode.Descendants("application"))
            {
                var name = applicationNode.Attribute("name").Value;
                var version = new Version(applicationNode.Attribute("version").Value);
                var application = new ApplicationDescription
                    {
                        Name = name,
                        Version = version,
                    };

                var storedApplicationId = m_Context.Applications()
                    .Where(a => a.IsNewerOrEqualVersionOf(application))
                    .OrderBy(a => a.Version)
                    .Select(a => a.Id)
                    .First();

                var testApplication = new TestApplication
                    {
                        ApplicationId = storedApplicationId,
                        TestEnvironmentId = environmentId,
                    };

                m_Context.Add(testApplication);
                m_Context.StoreChanges();
            }
        }
    }
}
