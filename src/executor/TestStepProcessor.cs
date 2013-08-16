//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Diagnostics;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Executor
{
    /// <summary>
    /// Defines the base class for types that process test steps.
    /// </summary>
    public abstract class TestStepProcessor : IProcessTestStep
    {
        /// <summary>
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </summary>
        private readonly RetrieveFileDataForTestStep m_TestFileLocation;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepProcessor"/> class.
        /// </summary>
        /// <param name="testFileLocation">
        /// The function that takes the name of the test step and returns the full path to the directory containing the files for the 
        /// given test step.
        /// </param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testFileLocation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        protected TestStepProcessor(RetrieveFileDataForTestStep testFileLocation, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => testFileLocation);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_TestFileLocation = testFileLocation;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Gets the object that provides the diagnostics methods for the application.
        /// </summary>
        protected SystemDiagnostics Diagnostics
        {
            get
            {
                return m_Diagnostics;
            }
        }

        /// <summary>
        /// Returns the full path to the directory which contains the files required by the current test step.
        /// </summary>
        /// <param name="stepIndex">The name of the test step.</param>
        /// <returns>The full path to the directory that contains the files for the current test step.</returns>
        protected string TestFileLocationFor(int stepIndex)
        {
            return m_TestFileLocation(stepIndex);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of test step that can be processed.
        /// </summary>
        public abstract Type TestTypeToProcess
        {
            get;
        }

        /// <summary>
        /// Processes the given test step.
        /// </summary>
        /// <param name="test">The test step that should be processed.</param>
        /// <param name="environmentParameters">The collection that provides the parameters for the environment.</param>
        /// <returns>The state of the test after the test step has been executed.</returns>
        public abstract TestExecutionState Process(TestStep test, IEnumerable<InputParameter> environmentParameters);
    }
}
