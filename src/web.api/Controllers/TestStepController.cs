//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web.Http;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api.Controllers
{
    /// <summary>
    /// The controller that takes care of the test step API calls.
    /// </summary>
    public sealed class TestStepController : ApiController
    {
        private readonly IProvideTestingContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public TestStepController(IProvideTestingContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
        }

        /// <summary>
        /// Registers a script executing test step.
        /// </summary>
        /// <param name="environment">The ID of the environment in which the step should be executed.</param>
        /// <param name="order">The order of the test step.</param>
        /// <param name="failureMode">
        ///     The failure mode which indicates if a test sequence should be stopped or continued if the current test step fails.
        /// </param>
        /// <param name="executablePath">The full path of the application that should be executed.</param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterConsole(int environment, int order, string failureMode, string executablePath)
        {
            var testStep = new ConsoleExecuteTestStep
            {
                Order = order,
                TestEnvironmentId = environment,
                FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                ExecutableFilePath = executablePath,
            };

            m_Context.Add(testStep);
            m_Context.StoreChanges();

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
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterMsi(int environment, int order, string failureMode)
        {
            var testStep = new MsiInstallTestStep
                {
                    Order = order,
                    TestEnvironmentId = environment,
                    FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                };

            m_Context.Add(testStep);
            m_Context.StoreChanges();

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
        /// <param name="language">The language of the script that should be executed.</param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterScript(int environment, int order, string failureMode, string language)
        {
            var testStep = new ScriptExecuteTestStep
                {
                    Order = order,
                    TestEnvironmentId = environment,
                    FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                    ScriptLanguage = (ScriptLanguage)Enum.Parse(typeof(ScriptLanguage), language),
                };

            m_Context.Add(testStep);
            m_Context.StoreChanges();

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
        /// <param name="destination">The full path to the destination folder where the files should be copied to.</param>
        /// <returns>The ID of the test step.</returns>
        [HttpPost]
        public int RegisterXCopy(int environment, int order, string failureMode, string destination)
        {
            var testStep = new XCopyTestStep
            {
                Order = order,
                TestEnvironmentId = environment,
                FailureMode = (TestStepFailureMode)Enum.Parse(typeof(TestStepFailureMode), failureMode),
                Destination = destination,
            };

            m_Context.Add(testStep);
            m_Context.StoreChanges();

            return testStep.Id;
        }
    }
}
