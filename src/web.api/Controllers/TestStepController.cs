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
    /// The controller that takes care of the test step API calls.
    /// </summary>
    public sealed class TestStepController : ApiController
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
        /// Initializes a new instance of the <see cref="TestStepController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TestStepController(IProvideTestingContext context, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => context);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Context = context;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Registers a script executing test step.
        /// </summary>
        /// <param name="environment">The ID of the environment in which the step should be executed.</param>
        /// <param name="order">The order of the test step.</param>
        /// <param name="failureMode">
        ///     The failure mode which indicates if a test sequence should be stopped or continued if the current test step fails.
        /// </param>
        /// <param name="shouldincludesystemlog">
        ///     A flag that indicates if the system log should be included in the report upon completion
        ///     of the current test step.
        /// </param>
        /// <param name="executablePath">The full path of the application that should be executed.</param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterConsole(
            int environment, 
            int order, 
            string failureMode,
            bool shouldincludesystemlog,
            string executablePath)
        {
            var testStep = new ConsoleExecuteTestStep
            {
                Order = order,
                TestEnvironmentId = environment,
                FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                ReportIncludesSystemLog = shouldincludesystemlog,
                ExecutableFilePath = executablePath,
            };

            try
            {
                m_Context.Add(testStep);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the console test step failed with error: {0}",
                        e));

                throw;
            }

            return testStep.Id;
        }

        /// <summary>
        /// Registers an MSI install test step.
        /// </summary>
        /// <param name="environment">The ID of the environment in which the step should be executed.</param>
        /// <param name="order">The order of the test step.</param>
        /// <param name="failureMode">
        ///     The failure mode which indicates if a test sequence should be stopped or continued if the current test step fails.
        /// </param>
        /// <param name="shouldincludesystemlog">
        ///     A flag that indicates if the system log should be included in the report upon completion
        ///     of the current test step.
        /// </param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterMsi(int environment, int order, string failureMode, bool shouldincludesystemlog)
        {
            var testStep = new MsiInstallTestStep
                {
                    Order = order,
                    TestEnvironmentId = environment,
                    FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                    ReportIncludesSystemLog = shouldincludesystemlog,
                };

            try
            {
                m_Context.Add(testStep);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the MSI test step failed with error: {0}",
                        e));

                throw;
            }

            return testStep.Id;
        }

        /// <summary>
        /// Registers a script executing test step.
        /// </summary>
        /// <param name="environment">The ID of the environment in which the step should be executed.</param>
        /// <param name="order">The order of the test step.</param>
        /// <param name="failureMode">
        ///     The failure mode which indicates if a test sequence should be stopped or continued if the current test step fails.
        /// </param>
        /// <param name="shouldincludesystemlog">
        ///     A flag that indicates if the system log should be included in the report upon completion
        ///     of the current test step.
        /// </param>
        /// <param name="language">The language of the script that should be executed.</param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterScript(int environment, int order, string failureMode, bool shouldincludesystemlog, string language)
        {
            var testStep = new ScriptExecuteTestStep
                {
                    Order = order,
                    TestEnvironmentId = environment,
                    FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                    ReportIncludesSystemLog = shouldincludesystemlog,
                    ScriptLanguage = (ScriptLanguage)Enum.Parse(typeof(ScriptLanguage), language),
                };

            try
            {
                m_Context.Add(testStep);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the script test step failed with error: {0}",
                        e));

                throw;
            }

            return testStep.Id;
        }

        /// <summary>
        /// Registers an x-copy test step.
        /// </summary>
        /// <param name="environment">The ID of the environment in which the step should be executed.</param>
        /// <param name="order">The order of the test step.</param>
        /// <param name="failureMode">
        ///     The failure mode which indicates if a test sequence should be stopped or continued if the current test step fails.
        /// </param>
        /// <param name="shouldincludesystemlog">
        ///     A flag that indicates if the system log should be included in the report upon completion
        ///     of the current test step.
        /// </param>
        /// <param name="destination">The full path to the destination folder where the files should be copied to.</param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterXCopy(int environment, int order, string failureMode, bool shouldincludesystemlog, string destination)
        {
            var testStep = new XCopyTestStep
            {
                Order = order,
                TestEnvironmentId = environment,
                FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                ReportIncludesSystemLog = shouldincludesystemlog,
                Destination = destination,
            };

            try
            {
                m_Context.Add(testStep);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the x-copy test step failed with error: {0}",
                        e));

                throw;
            }

            return testStep.Id;
        }
    }
}
