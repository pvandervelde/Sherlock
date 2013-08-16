//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Executor
{
    /// <summary>
    /// Stores the information for the currently active test.
    /// </summary>
    internal sealed class ActiveTestInformation
    {
        /// <summary>
        /// Gets or sets the collection of test steps that should be executed.
        /// </summary>
        public List<TestStep> TestSteps
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the full path to the test package.
        /// </summary>
        public string TestPackage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection containing the parameters describing the global environment for the test.
        /// </summary>
        public List<InputParameter> EnvironmentParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current state of the test.
        /// </summary>
        public TestExecutionState CurrentState
        {
            get;
            set;
        }
    }
}
