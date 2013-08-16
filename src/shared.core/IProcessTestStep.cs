//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that process <see cref="TestStep"/> instances.
    /// </summary>
    public interface IProcessTestStep
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of test step that can be processed.
        /// </summary>
        Type TestTypeToProcess
        {
            get;
        }

        /// <summary>
        /// Processes the given test step.
        /// </summary>
        /// <param name="test">The test step that should be processed.</param>
        /// <param name="environmentParameters">The collection that provides the parameters for the environment.</param>
        /// <returns>The state of the test after the test step has been executed.</returns>
        TestExecutionState Process(TestStep test, IEnumerable<InputParameter> environmentParameters);
    }
}
