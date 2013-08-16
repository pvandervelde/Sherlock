//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that provide access to an active environment.
    /// </summary>
    public interface IActiveEnvironment
    {
        /// <summary>
        /// Gets the ID of the environment specification that is used to create
        /// the current active environment instance.
        /// </summary>
        string Environment
        {
            get;
        }

        /// <summary>
        /// Gets or sets the number of times the environment failed to respond to a keep-alive
        /// contact.
        /// </summary>
        int NumberOfKeepAliveFailures 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the ID of the currently running test.
        /// </summary>
        int RunningTest
        {
            get;
        }

        /// <summary>
        /// Returns the current state of the environment.
        /// </summary>
        /// <param name="token">The cancellation token that can be used to cancel the gathering of the state.</param>
        /// <returns>A task that will return the current state of the environment.</returns>
        Task<EnvironmentState> State(CancellationToken token);

        /// <summary>
        /// Executes the specific set of tests on the environment.
        /// </summary>
        /// <param name="testId">The ID of the test that is being executed.</param>
        /// <param name="testSteps">The collection of test steps that should be executed.</param>
        /// <param name="environmentParameters">The collection that contains the parameters related to the test environment.</param>
        /// <param name="testPackageFile">The full path to the package file that contains all the test files for the given test.</param>
        void Execute(int testId, IEnumerable<TestStep> testSteps, IEnumerable<InputParameter> environmentParameters, string testPackageFile);

        /// <summary>
        /// An event raised when progress has been made in the test.
        /// </summary>
        event EventHandler<TestExecutionProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// An event raised when the test execution has completed.
        /// </summary>
        event EventHandler<TestExecutionResultEventArgs> OnTestCompletion;

        /// <summary>
        /// Stops the currently running test.
        /// </summary>
        void Terminate();

        /// <summary>
        /// Terminates the active environment.
        /// </summary>
        /// <remarks>
        /// This will also terminate any running tests, but the test results will not be send out.
        /// </remarks>
        void Shutdown();
    }
}
