//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nuclei.Communication;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for commands that provide the ability to execute test cases.
    /// </summary>
    public interface IExecuteTestStepsCommands : ICommandSet
    {
        /// <summary>
        /// Starts the execution of the given test steps.
        /// </summary>
        /// <param name="testId">The ID of the test that is being executed.</param>
        /// <param name="testSteps">The collection of test steps that should be executed.</param>
        /// <param name="environmentParameters">The collection that contains the parameters related to the test environment.</param>
        /// <param name="callingEndpoint">The ID of the endpoint that called the method.</param>
        /// <param name="token">The upload token that can be used to upload the file.</param>
        /// <returns>A task which completes when the execution has started successfully.</returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
            Justification = "Really want this to be a 'list' because that stops lazy evaluation.")]
        Task Execute(
            int testId,
            List<TestStep> testSteps,
            List<InputParameter> environmentParameters, 
            EndpointId callingEndpoint, 
            UploadToken token);

        /// <summary>
        /// Returns a value indicating the current state of the test.
        /// </summary>
        /// <returns>A task that returns the current state of the test.</returns>
        Task<TestExecutionState> State();

        /// <summary>
        /// Stops the execution of the currently running test.
        /// </summary>
        /// <returns>A task that completes when the stop process is finished.</returns>
        Task Terminate();
    }
}
