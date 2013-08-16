//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Web.Http;
using System.Xml.Linq;
using Nuclei.Configuration;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api.Controllers
{
    /// <summary>
    /// The controller that takes care of the test API calls.
    /// </summary>
    public sealed class TestController : ApiController
    {
        private readonly IConfiguration m_Configuration;

        private readonly IProvideTestingContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public TestController(IConfiguration configuration, IProvideTestingContext context)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => context);
            }

            m_Configuration = configuration;
            m_Context = context;
        }

        /// <summary>
        /// Registers a new test and returns the ID of the new test.
        /// </summary>
        /// <param name="product">The name of the product.</param>
        /// <param name="version">The version of the product.</param>
        /// <param name="owner">The username of the user that is registering the test.</param>
        /// <param name="description">The description for the test.</param>
        /// <param name="reportPath">The full path to the location where the test results are placed.</param>
        /// <returns>An XML document containing the ID of the new test.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="product"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="product"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="version"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="owner"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="reportPath"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="reportPath"/> is an empty string.
        /// </exception>
        [HttpPost]
        public int Register(
            string product, 
            string version, 
            string owner, 
            string description, 
            string reportPath)
        {
            {
                Lokad.Enforce.Argument(() => product);
                Lokad.Enforce.Argument(() => product, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => version, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => owner, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => reportPath);
                Lokad.Enforce.Argument(() => reportPath, Lokad.Rules.StringIs.NotEmpty);
            }

            var test = new Test
                {
                    ProductName = product,
                    ProductVersion = version,
                    Owner = owner,
                    TestDescription = description,
                    ReportPath = reportPath
                };

            m_Context.Add(test);
            m_Context.StoreChanges();

            return test.Id;
        }

        /// <summary>
        /// Uploads the test files.
        /// </summary>
        /// <param name="id">The ID of the test to which the test files belong.</param>
        [HttpPost]
        public void Upload(int id)
        {
            var content = Request.Content;
            var streamContent = content.ReadAsStreamAsync().Result;

            var testFile = Path.Combine(
                m_Configuration.Value<string>(WebApiConfigurationKeys.TestDataDirectory),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.zip",
                    id));
            using (var fileStream = new FileStream(testFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                streamContent.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Indicate that the given test is ready for execution.
        /// </summary>
        /// <param name="id">The ID of the test.</param>
        [HttpPost]
        public void MarkAsReady(int id)
        {
            m_Context.TestIsReadyForExecution(id);
        }
    }
}
