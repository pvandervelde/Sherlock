//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Web.Http;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api.Controllers
{
    /// <summary>
    /// A controller that takes care of the test step additional report files API calls.
    /// </summary>
    public sealed class TestStepAdditionalReportFilesController : ApiController
    {
        /// <summary>
        /// The database context.
        /// </summary>
        private readonly IProvideTestingContext m_Context;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepAdditionalReportFilesController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TestStepAdditionalReportFilesController(IProvideTestingContext context, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => context);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Context = context;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Registers a file path for a file that should be included in the report.
        /// </summary>
        /// <param name="testStep">The ID of the test step which generates the file.</param>
        /// <param name="path">The full path of the file that should be included in the report.</param>
        [System.Web.Mvc.HttpPost]
        public void ForFile(int testStep, string path)
        {
            var testStepReportFile = new TestStepReportFile
            {
                TestStepId = testStep,
                Path = path,
            };

            try
            {
                m_Context.Add(testStepReportFile);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the test step report files failed with error: {0}",
                        e));

                throw;
            }
        }

        /// <summary>
        /// Registers a directory path for a directory that should be included in the report.
        /// </summary>
        /// <param name="testStep">The ID of the test step which generates the files in the directory.</param>
        /// <param name="path">The full path of the directory that should be included in the report.</param>
        [System.Web.Mvc.HttpPost]
        public void ForDirectory(int testStep, string path)
        {
            var testStepReportDirectory = new TestStepReportDirectory
            {
                TestStepId = testStep,
                Path = path,
            };

            try
            {
                m_Context.Add(testStepReportDirectory);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the test step report files failed with error: {0}",
                        e));

                throw;
            }
        }
    }
}
