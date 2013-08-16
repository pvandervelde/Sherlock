//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nuclei.Communication;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.DataAccess;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the base class for active environments that form a proxy for a machine.
    /// </summary>
    internal abstract class ActiveMachineEnvironment : IActiveEnvironment
    {
        /// <summary>
        /// The commands that are used to control the remote environment.
        /// </summary>
        private readonly IExecuteTestStepsCommands m_Commands;

        /// <summary>
        /// The object that provides the notifications from the remote environment.
        /// </summary>
        private readonly ITestExecutionNotifications m_Notifications;

        /// <summary>
        /// The object that stores upload references.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// The ID of the environment.
        /// </summary>
        private readonly string m_Id;

        /// <summary>
        /// The action that is used to terminate the remote environment.
        /// </summary>
        private readonly Action m_TerminateEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMachineEnvironment"/> class.
        /// </summary>
        /// <param name="id">The ID of the environment.</param>
        /// <param name="terminateEnvironment">The action used to terminate the environment.</param>
        /// <param name="commands">The object that provides the commands used to communicate with the environment.</param>
        /// <param name="notifications">The object that provides notifications from the environment.</param>
        /// <param name="uploads">The object that tracks the files available for upload.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="terminateEnvironment"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="uploads"/> is <see langword="null" />.
        /// </exception>
        protected ActiveMachineEnvironment(
            string id, 
            Action terminateEnvironment, 
            IExecuteTestStepsCommands commands, 
            ITestExecutionNotifications notifications,
            IStoreUploads uploads)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => terminateEnvironment);
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.Argument(() => uploads);
            }

            m_Id = id;
            m_TerminateEnvironment = terminateEnvironment;
            m_Commands = commands;
            m_Uploads = uploads;
            
            m_Notifications = notifications;
            m_Notifications.OnExecutionProgress += HandleOnExecutionProgress;
            m_Notifications.OnTestCompletion += HandleOnTestCompletion;
        }

        private void HandleOnExecutionProgress(object sender, TestExecutionProgressEventArgs eventArgs)
        {
            var local = OnExecutionProgress;
            if (local != null)
            {
                // The remote environments don't actually know the test ID, but we do ...
                local(this, new TestExecutionProgressEventArgs(RunningTest, eventArgs.SectionName, eventArgs.Section));
            }
        }

        private void HandleOnTestCompletion(object sender, TestExecutionResultEventArgs eventArgs)
        {
            var local = OnTestCompletion;
            if (local != null)
            {
                // The remote environments don't actually know the test ID, but we do ...
                local(this, new TestExecutionResultEventArgs(RunningTest, eventArgs.Result));
            }
        }

        /// <summary>
        /// Gets the ID of the environment specification that is used to create
        /// the current active environment instance.
        /// </summary>
        public string Environment
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets or sets the number of times the environment failed to respond to a keep-alive
        /// contact.
        /// </summary>
        public int NumberOfKeepAliveFailures
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ID of the currently running test.
        /// </summary>
        public int RunningTest
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the current state of the environment.
        /// </summary>
        /// <param name="token">The cancellation token that can be used to cancel the gathering of the state.</param>
        /// <returns>A task that will return the current state of the environment.</returns>
        public Task<EnvironmentState> State(CancellationToken token)
        {
            var result = Task<EnvironmentState>.Factory.StartNew(
                () =>
                    {
                        var task = m_Commands.State();

                        TestExecutionState state;
                        try
                        {
                            task.Wait(token);
                            state = task.Result;
                        }
                        catch (AggregateException)
                        {
                            state = TestExecutionState.Unknown;
                        }

                        return new EnvironmentState(state);
                    });

            return result;
        }

        /// <summary>
        /// Executes the specific set of tests on the environment.
        /// </summary>
        /// <param name="testId">The ID of the test that is being executed.</param>
        /// <param name="testSteps">The test cases that should be executed.</param>
        /// <param name="environmentParameters">The collection that contains the parameters related to the test environment.</param>
        /// <param name="testPackageFile">The full path to the package file that contains all the test files for the given test.</param>
        public void Execute(int testId, IEnumerable<TestStep> testSteps, IEnumerable<InputParameter> environmentParameters, string testPackageFile)
        {
            RunningTest = testId;

            var token = m_Uploads.Register(testPackageFile);
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var result = m_Commands.Execute(testSteps.ToList(), environmentParameters.ToList(), endpoint, token);
            try
            {
                result.Wait();
            }
            catch (AggregateException e)
            {
                throw new TestExecutionFailureException(
                    Resources.Exceptions_Messages_TestExecutionFailure,
                    e);
            }
        }

        /// <summary>
        /// An event raised when progress has been made in the test.
        /// </summary>
        public event EventHandler<TestExecutionProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// An event raised when the test execution has completed.
        /// </summary>
        public event EventHandler<TestExecutionResultEventArgs> OnTestCompletion;

        /// <summary>
        /// Stops the currently running test.
        /// </summary>
        public void Terminate()
        {
            var result = m_Commands.Terminate();
            try
            {
                result.Wait();
            }
            catch (AggregateException)
            {
                // Just ignore it for now
            }
        }

        /// <summary>
        /// Terminates the active environment.
        /// </summary>
        /// <remarks>
        /// This will also terminate any running tests, but the test results will not be send out.
        /// </remarks>
        public void Shutdown()
        {
            m_TerminateEnvironment();
        }
    }
}
