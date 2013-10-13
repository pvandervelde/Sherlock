//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Sherlock.Console
{
    /// <summary>
    /// Defines the base for the description of a test step.
    /// </summary>
    internal abstract class TestStepDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStepDescription"/> class.
        /// </summary>
        /// <param name="environment">The name of the environment to which the current step belongs.</param>
        /// <param name="order">The index of the test step in the test sequence.</param>
        /// <param name="failureMode">The failure mode that describes what action should be taken if the current test step fails.</param>
        /// <param name="parameters">The collection containing the parameters for the current test step.</param>
        /// <param name="transferLogFileOnStepComplete">
        ///     A flag that indicates whether the system log file should be transferred back to the host once the current
        ///     test step is completed.
        /// </param>
        /// <param name="elementsToTransferOnTestStepComplete">
        ///     The collection containing the file and directory paths that should be transferred back to the host once the current
        ///     test step is completed.
        /// </param>
        protected TestStepDescription(
            string environment, 
            int order, 
            string failureMode,
            IEnumerable<TestStepParameterDescription> parameters,
            bool transferLogFileOnStepComplete,
            IEnumerable<FileSystemInfo> elementsToTransferOnTestStepComplete)
        {
            {
                Debug.Assert(!string.IsNullOrEmpty(environment), "The name of the environment should not be an empty string.");
                Debug.Assert(order >= 0, "The order of the test step should be a postive integer.");
                Debug.Assert(!string.IsNullOrEmpty(failureMode), "The failure mode should not be an empty string.");
                Debug.Assert(parameters != null, "The parameters collection should not be a null reference.");
                Debug.Assert(elementsToTransferOnTestStepComplete != null, "The transfer elements collection should not be a null reference.");
            }

            Environment = environment;
            Order = order;
            FailureMode = failureMode;
            Parameters = parameters;
            TransferLogFileOnStepComplete = transferLogFileOnStepComplete;
            ElementsToTransferOnTestStepComplete = elementsToTransferOnTestStepComplete;
        }

        /// <summary>
        /// Gets the name of the environment on which the test step should be executed.
        /// </summary>
        public string Environment
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the order of the test step in the test sequence.
        /// </summary>
        public int Order
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the failure mode that describes if the test sequence should continue or stop if the current
        /// test step fails.
        /// </summary>
        public string FailureMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection containing the parameters for the current test step.
        /// </summary>
        public IEnumerable<TestStepParameterDescription> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the system log file should be copied back to the host once the current
        /// test step is completed.
        /// </summary>
        public bool TransferLogFileOnStepComplete
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection containing the paths for all files and directories that should be copied back to the host
        /// once the current test step is completed.
        /// </summary>
        public IEnumerable<FileSystemInfo> ElementsToTransferOnTestStepComplete
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the ID of the test step.
        /// </summary>
        public int Id
        {
            get;
            set;
        }
    }
}
