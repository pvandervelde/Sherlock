//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Timers;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Controls the test cycle from the processing of the test request to keeping the test environments alive.
    /// </summary>
    internal sealed class TestCycle : ICycleTestsFromRequestToCompletion, IDisposable
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The timer that is used to periodically check that all the test environments are still running.
        /// </summary>
        private readonly System.Timers.Timer m_Timer = new System.Timers.Timer();

        /// <summary>
        /// The object that stores the tests that are currently being executed.
        /// </summary>
        private readonly IStoreActiveTests m_ExecutingTests;

        /// <summary>
        /// The object that handles the test execution.
        /// </summary>
        private readonly IControlTests m_TestController;

        /// <summary>
        /// The object that provides diagnostics methods.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The current time between two activities in the test cycle in milliseconds.
        /// </summary>
        private readonly int m_CycleTimeInMilliSeconds;

        /// <summary>
        /// The maximum number of times an environment can miss a cycle.
        /// </summary>
        private readonly int m_MaximumNumberOfCycleFailures;

        /// <summary>
        /// This is the synchronization point that prevents events
        /// from running concurrently, and prevents the main thread 
        /// from executing code after the Stop method until any 
        /// event handlers are done executing.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private int m_SyncPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCycle"/> class.
        /// </summary>
        /// <param name="configuration">The object that provides access to the configuration options for the application.</param>
        /// <param name="testController">The object that handles test execution.</param>
        /// <param name="executingTests">The object that stores information about the test that are currently being executed.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testController"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="executingTests"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public TestCycle(
            IConfiguration configuration,
            IControlTests testController,
            IStoreActiveTests executingTests,
            SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => testController);
                Lokad.Enforce.Argument(() => executingTests);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_TestController = testController;
            m_Diagnostics = diagnostics;

            m_ExecutingTests = executingTests;

            m_CycleTimeInMilliSeconds = configuration.HasValueFor(MasterServiceConfigurationKeys.KeepAliveCycleTimeInMilliSeconds)
                ? configuration.Value<int>(MasterServiceConfigurationKeys.KeepAliveCycleTimeInMilliSeconds)
                : GlobalConstants.DefaultKeepAliveCycleTimeInMilliseconds;
            m_MaximumNumberOfCycleFailures = configuration.HasValueFor(MasterServiceConfigurationKeys.MaximumNumberOfCycleFailures)
                ? configuration.Value<int>(MasterServiceConfigurationKeys.MaximumNumberOfCycleFailures)
                : GlobalConstants.DefaultMaximumNumberOfCycleFailures;

            m_Timer.AutoReset = true;
            m_Timer.Interval = m_CycleTimeInMilliSeconds;
            m_Timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Processes the elapsed event of the progress timer.
        /// </summary>
        /// <param name="sender">The timer at which the event originated.</param>
        /// <param name="e">The event arguments.</param>
        /// <design>
        /// <para>
        /// This code assumes that overlapping events can be
        /// discarded. That is, if an TimerElapsed event is raised before 
        /// the previous event is finished processing, the second
        /// event is ignored. In this case that is probably not 
        /// directly what we want however we push all the event
        /// processing onto a seperate thread so the execution of
        /// the event should not take very long compared to the 
        /// timer interval.
        /// </para>
        /// </design>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // CompareExchange is used to take control of m_SyncPoint, 
            // and to determine whether the attempt was successful. 
            // CompareExchange attempts to put 1 into syncPoint, but
            // only if the current value of syncPoint is zero 
            // (specified by the third parameter). If another thread
            // has set syncPoint to 1, or if the control thread has
            // set syncPoint to -1, the current event is skipped. 
            if (Interlocked.CompareExchange(ref m_SyncPoint, 1, 0) == 0)
            {
                ThreadPool.QueueUserWorkItem(
                    state =>
                    {
                        try
                        {
                            // Check all the queued tests to see if they can be executed or not (depends on the available test environments)
                            m_TestController.ActivateTests();

                            // Check that all active environments are still active (they respond to a ping on the environment AND on the service)
                            PingActiveEnvironments();
                        }
                        catch (Exception exception)
                        {
                            m_Diagnostics.Log(
                                LevelToLog.Error, 
                                MasterServiceConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_TestActivationFailed_WithError,
                                    exception));
                        }
                    });

                // Release control of syncPoint
                // We can just write to the value because integers 
                // are atomically written.
                // 
                // On top of that we only use this variable internally and
                // we'll never do anything with it if the value is unequal to 0.
                m_SyncPoint = 0;
            }
        }

        private void PingActiveEnvironments()
        {
            var activeEnvironments = new List<IActiveEnvironment>();
            lock (m_Lock)
            {
                foreach (var testId in m_ExecutingTests)
                {
                    activeEnvironments.AddRange(m_ExecutingTests.EnvironmentsForTest(testId));
                }
            }

            foreach (var environment in activeEnvironments)
            {
                if (environment.HasBeenTerminated)
                {
                    // Only check those environments that haven't been marked as terminated.
                    // Environments that are terminated should either be shut down or in the
                    // process of shutting down.
                    continue;
                }

                var local = environment;
                var tokenSource = new CancellationTokenSource(m_CycleTimeInMilliSeconds);
                local.State(tokenSource.Token)
                    .ContinueWith(
                        t =>
                        {
                            if (t.Exception != null)
                            {
                                m_Diagnostics.Log(
                                    LevelToLog.Error,
                                    MasterServiceConstants.LogPrefix,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        Resources.Log_Messages_FailedToContactEnvironment_WithException,
                                        local.Environment,
                                        t.Exception));

                                local.NumberOfKeepAliveFailures += 1;
                                return;
                            }

                            if (t.IsCanceled)
                            {
                                m_Diagnostics.Log(
                                    LevelToLog.Warn,
                                    MasterServiceConstants.LogPrefix,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        Resources.Log_Messages_FailedToContactEnvironment,
                                        local.Environment));

                                local.NumberOfKeepAliveFailures += 1;
                                return;
                            }

                            var state = t.Result;
                            if (state.TestState == TestExecutionState.Unknown)
                            {
                                m_Diagnostics.Log(
                                    LevelToLog.Warn,
                                    MasterServiceConstants.LogPrefix,
                                    string.Format(
                                        CultureInfo.InvariantCulture,
                                        Resources.Log_Messages_FailedToContactEnvironment,
                                        local.Environment));

                                local.NumberOfKeepAliveFailures += 1;
                                return;
                            }

                            m_Diagnostics.Log(
                                LevelToLog.Debug,
                                MasterServiceConstants.LogPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.Log_Messages_ProgressFromEnvironment,
                                    local.Environment,
                                    t.Result));
                            local.NumberOfKeepAliveFailures = 0;
                        })
                    .ContinueWith(
                        t =>
                        {
                            if (local.NumberOfKeepAliveFailures > m_MaximumNumberOfCycleFailures)
                            {
                                // The environment is stuffed, may as well give up.
                                m_ExecutingTests.EnvironmentFailure(local.RunningTest, local.Environment);
                            }
                        });
            }
        }

        /// <summary>
        /// Stops the progress timer so that all currently running events
        /// are finished.
        /// </summary>
        /// <source>
        /// http://msdn.microsoft.com/en-us/library/system.timers.timer.stop.aspx
        /// </source>
        private void StopTimer()
        {
            m_Timer.Stop();

            // Ensure that if an event is currently executing,
            // no further processing is done on this thread until
            // the event handler is finished. This is accomplished
            // by using CompareExchange to place -1 in syncPoint,
            // but only if syncPoint is currently zero (specified
            // by the third parameter of CompareExchange). 
            // CompareExchange returns the original value that was
            // in syncPoint. If it was not zero, then there's an
            // event handler running, and it is necessary to try
            // again.
            while (Interlocked.CompareExchange(ref m_SyncPoint, -1, 0) != 0)
            {
                // Give up the rest of this thread's current time
                // slice.
                Thread.Yield();
            }

            // Any processing done after this point does not conflict
            // with timer events. This is the purpose of the call to
            // CompareExchange. If the processing done here would not
            // cause a problem when run concurrently with timer events,
            // then there is no need for the extra synchronization.
        }

        /// <summary>
        /// Starts the test cycle.
        /// </summary>
        public void Start()
        {
            m_SyncPoint = 0;
            m_Timer.Start();
        }

        /// <summary>
        /// Stops the test cycle.
        /// </summary>
        public void Stop()
        {
            StopTimer();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IDisposable disposable = m_Timer;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
