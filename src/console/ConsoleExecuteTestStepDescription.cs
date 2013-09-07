//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores a description of a test step that executes a console application.
    /// </summary>
    internal sealed class ConsoleExecuteTestStepDescription : TestStepDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleExecuteTestStepDescription"/> class.
        /// </summary>
        /// <param name="environment">The name of the environment to which the current step belongs.</param>
        /// <param name="order">The index of the test step in the test sequence.</param>
        /// <param name="failureMode">The failure mode that describes what action should be taken if the current test step fails.</param>
        /// <param name="parameters">The collection containing the parameters for the current test step.</param>
        /// <param name="executablePath">The full path to the executable that needs to be run during the current test step.</param>
        public ConsoleExecuteTestStepDescription(
            string environment, 
            int order, 
            string failureMode, 
            IEnumerable<TestStepParameterDescription> parameters,
            string executablePath) 
            : base(environment, order, failureMode, parameters)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(executablePath), "The executable path should not be an empty string.");
            }

            ExecutablePath = executablePath;
        }

        /// <summary>
        /// Gets the full path to the executable that should be run during the current test step.
        /// </summary>
        public string ExecutablePath
        {
            get;
            private set;
        }
    }
}
