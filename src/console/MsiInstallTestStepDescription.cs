//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Sherlock.Console
{
    /// <summary>
    /// Stores a description of an MSI install test step.
    /// </summary>
    internal sealed class MsiInstallTestStepDescription : TestStepDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsiInstallTestStepDescription"/> class.
        /// </summary>
        /// <param name="environment">The name of the environment to which the current step belongs.</param>
        /// <param name="order">The index of the test step in the test sequence.</param>
        /// <param name="failureMode">The failure mode that describes what action should be taken if the current test step fails.</param>
        /// <param name="parameters">The collection containing the parameters for the current test step.</param>
        public MsiInstallTestStepDescription(
            string environment, 
            int order,
            string failureMode,
            IEnumerable<TestStepParameterDescription> parameters) 
            : base(environment, order, failureMode, parameters)
        {
        }
    }
}
