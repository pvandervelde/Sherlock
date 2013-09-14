//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.Http;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Web.Api.Controllers
{
    /// <summary>
    /// A controller that takes care of the test step parameter API calls.
    /// </summary>
    public sealed class TestStepParameterController : ApiController
    {
        private readonly IProvideTestingContext m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepParameterController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public TestStepParameterController(IProvideTestingContext context)
        {
            {
                Lokad.Enforce.Argument(() => context);
            }

            m_Context = context;
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
                Trace.TraceError(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Registering the test step parameter failed with error: {0}",
                        e));

                throw;
            }
        }
    }
}
