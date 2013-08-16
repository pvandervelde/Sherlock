//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web.Http;
using System.Xml.Linq;
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
        /// Registers an MSI install test step.
        /// </summary>
        /// <param name="environment">The ID of the environment in which the step should be executed.</param>
        /// <param name="order">The order of the test step.</param>
        /// <returns>An XML document containing the ID of the new test step.</returns>
        [HttpPost]
        public int RegisterMsi(int environment, int order)
        {
            var testStep = new MsiInstallTestStep
                {
                    Order = order,
                    TestEnvironmentId = environment,
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
        /// <param name="language">The language of the script that should be executed.</param>
        /// <returns>An XML document containing the ID of the new test step.</returns>
        [HttpPost]
        public int RegisterScript(int environment, int order, string language)
        {
            var testStep = new ScriptExecuteTestStep
                {
                    Order = order,
                    TestEnvironmentId = environment,
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
        /// <param name="destination">The full path to the destination folder where the files should be copied to.</param>
        /// <returns>An XML document containing the ID of the new test step.</returns>
        [HttpPost]
        public int RegisterXCopy(int environment, int order, string destination)
        {
            var testStep = new XCopyTestStep
            {
                Order = order,
                TestEnvironmentId = environment,
                Destination = destination,
            };

            m_Context.Add(testStep);
            m_Context.StoreChanges();

            return testStep.Id;
        }
    }
}
