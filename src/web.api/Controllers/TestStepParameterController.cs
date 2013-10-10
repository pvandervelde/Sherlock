//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.Http;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api.Controllers
{
    /// <summary>
    /// A controller that takes care of the test step parameter API calls.
    /// </summary>
    public sealed class TestStepParameterController : ApiController
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
        /// Initializes a new instance of the <see cref="TestStepParameterController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TestStepParameterController(IProvideTestingContext context, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => context);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Context = context;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Registers a test step parameter.
        /// </summary>
        /// <param name="testStep">The ID of the test step to which the parameter belongs.</param>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="key"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="key"/> is an empty string.
        /// </exception>
        public void Register(int testStep, string key, string value)
        {
            {
                Lokad.Enforce.Argument(() => key);
                Lokad.Enforce.Argument(() => key, Lokad.Rules.StringIs.NotEmpty);
            }

            var parameter = new TestStepParameter
                {
                    Key = key,
                    Value = value,
                    TestStepId = testStep,
                };

            try
            {
                m_Context.Add(parameter);
                m_Context.StoreChanges();
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    WebApiConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the test step parameter failed with error: {0}",
                        e));

                throw;
            }
        }
    }
}
