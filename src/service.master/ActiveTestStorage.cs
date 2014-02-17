//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Master
{
    internal sealed class ActiveTestStorage : IStoreActiveTests
    {
        /// <summary>
        /// Indicates in what state the test execution on a given environmentId is.
        /// </summary>
        private enum EnvironmentTestExecutionState
        {
            /// <summary>
            /// The test is in an unknown state.
            /// </summary>
            None,

            /// <summary>
            /// The test is executing.
            /// </summary>
            Executing,

            /// <summary>
            /// The test has completed executing.
            /// </summary>
            Complete
        }

        /// <summary>
        /// Stores information about an active test.
        /// </summary>
        private sealed class TestMap
        {
            /// <summary>
            /// The collection of environments that are used to execute a test.
            /// </summary>
            private readonly List<Tuple<IActiveEnvironment, EnvironmentTestExecutionState>> m_Environments
                = new List<Tuple<IActiveEnvironment, EnvironmentTestExecutionState>>();

            /// <summary>
            /// The collection containing all the completion notifications for the test.
            /// </summary>
            private readonly List<TestCompletedNotification> m_Notifications;

            /// <summary>
            /// The report builder for the test.
            /// </summary>
            private readonly IReportBuilder m_Reports;

            /// <summary>
            /// Initializes a new instance of the <see cref="TestMap"/> class.
            /// </summary>
            /// <param name="reports">The report builder for the test.</param>
            /// <param name="notifications">The collection test completed notifications for the test.</param>
            public TestMap(IReportBuilder reports, IEnumerable<TestCompletedNotification> notifications)
            {
                {
                    Debug.Assert(reports != null, "The report builder should not be a null reference.");
                    Debug.Assert(notifications != null, "The notifications collection should not be a null reference.");
                }

                m_Reports = reports;
                m_Notifications = new List<TestCompletedNotification>(notifications);
            }

            /// <summary>
            /// Gets the report builder for the test.
            /// </summary>
            public IReportBuilder Reports
            {
                get
                {
                    return m_Reports;
                }
            }

            /// <summary>
            /// Gets the collection containing all the environments that are being used to execute the test.
            /// </summary>
            public List<Tuple<IActiveEnvironment, EnvironmentTestExecutionState>> Environments
            {
                get
                {
                    return m_Environments;
                }
            }

            /// <summary>
            /// Gets the collection containing all the test completion notifications for the current test.
            /// </summary>
            public List<TestCompletedNotification> Notifications
            {
                get
                {
                    return m_Notifications;
                }
            }
        }

        private static TestSection EnvironmentFailedSection(string environmentName)
        {
            var section = new TestSection(
                Resources.ReportSection_Name_Environments,
                DateTimeOffset.Now,
                DateTimeOffset.Now,
                false,
                new List<DateBasedTestInformation>(),
                new List<DateBasedTestInformation>(),
                new List<DateBasedTestInformation>
                    {
                        new DateBasedTestInformation(
                            DateTimeOffset.Now, 
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.ReportSection_Error_LostContactWithEnvironment_WithEnvironment,
                                environmentName))
                    });

            return section;
        }

        /// <summary>
        /// The collection of active tests.
        /// </summary>
        private readonly IDictionary<int, TestMap> m_Tests
            = new Dictionary<int, TestMap>();

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveTestStorage"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public ActiveTestStorage(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Adds a test to the collection and associates it with a report builder.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="builder">The report builder that will generate the report for the test.</param>
        /// <param name="notifications">The collection of test complete notifications for the test.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notifications"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if there already is a registered test with the <paramref name="test"/> ID.
        /// </exception>
        public void Add(int test, IReportBuilder builder, IEnumerable<TestCompletedNotification> notifications)
        {
            {
                Lokad.Enforce.Argument(() => builder);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.With<ArgumentException>(
                    !m_Tests.ContainsKey(test),
                    Resources.Exceptions_Messages_ATestWithTheGivenIDHasAlreadyBeenRegistered);
            }

            m_Tests.Add(test, new TestMap(builder, notifications));
        }

        /// <summary>
        /// Associates an environmentId with a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <param name="environment">The environmentId.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="environment"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if there is no registered test with the <paramref name="test"/> ID.
        /// </exception>
        public void AddEnvironmentForTest(int test, IActiveEnvironment environment)
        {
            {
                Lokad.Enforce.Argument(() => environment);

                Lokad.Enforce.With<ArgumentException>(
                    m_Tests.ContainsKey(test),
                    Resources.Exceptions_Messages_NoTestWithTheGivenIDHasBeenRegistered);
            }

            var collection = m_Tests[test].Environments;
            collection.Add(new Tuple<IActiveEnvironment, EnvironmentTestExecutionState>(environment, EnvironmentTestExecutionState.Executing));

            environment.OnExecutionProgress += HandleOnExecutionProgress;
            environment.OnTestCompletion += HandleOnTestCompletion;
        }

        private void HandleOnExecutionProgress(object sender, TestExecutionProgressEventArgs e)
        {
            var map = m_Tests[e.Id];
            map.Reports.AddToSection(
                e.SectionName,
                new List<TestSection>
                    {
                        e.Section
                    });
        }

        private void HandleOnTestCompletion(object sender, TestExecutionResultEventArgs e)
        {
            var environment = sender as IActiveEnvironment;

            var map = m_Tests[e.Id];
            var index = map.Environments.FindIndex(t => t.Item1.Environment == environment.Environment);
            map.Environments[index] = new Tuple<IActiveEnvironment, EnvironmentTestExecutionState>(
                environment, 
                EnvironmentTestExecutionState.Complete);

            if (map.Environments.All(t => t.Item2 == EnvironmentTestExecutionState.Complete))
            {
                CompleteTest(e.Id, e.Result);
            }
        }

        private void CompleteTest(int test, TestExecutionResult result)
        {
            RaiseOnTestCompletion(test, result);
        }

        private void RaiseOnTestCompletion(int id, TestExecutionResult result)
        {
            var local = OnTestCompletion;
            if (local != null)
            {
                local(this, new TestExecutionResultEventArgs(id, result));
            }
        }

        /// <summary>
        /// Removes the test with the given ID from the storage.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        public void Remove(int test)
        {
            if (m_Tests.ContainsKey(test))
            {
                var map = m_Tests[test];
                foreach (var environment in map.Environments)
                {
                    environment.Item1.OnExecutionProgress -= HandleOnExecutionProgress;
                    environment.Item1.OnTestCompletion -= HandleOnTestCompletion;
                }

                m_Tests.Remove(test);
            }
        }

        /// <summary>
        /// Indicates that the specific environmentId has failed to return a response and is probably deceased.
        /// </summary>
        /// <param name="runningTest">The ID of the test that is running on the environment.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="environmentId"/> is <see langword="null" />.
        /// </exception>
        public void EnvironmentFailure(int runningTest, string environmentId)
        {
            {
                Lokad.Enforce.Argument(() => environmentId);
            }

            if (!m_Tests.ContainsKey(runningTest))
            {
                return;
            }

            var map = m_Tests[runningTest];
            if (map.Environments.FindIndex(t => t.Item1.Environment == environmentId) == -1)
            {
                return;
            }

            // Stop all the environments
            foreach (var environment in map.Environments)
            {
                try
                {
                    environment.Item1.Terminate();
                }
                catch (Exception e)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error, 
                        MasterServiceConstants.LogPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Log_Messages_FailedToTerminateEnvironmentOnFailure_WithFailedEnvironmentAndError,
                            environment.Item1.Environment,
                            environmentId,
                            e));
                }
            }

            map.Reports.AddToSection(
                Resources.ReportSection_Name_Environments,
                new List<TestSection>
                    {
                        EnvironmentFailedSection(environmentId),
                    });
            RaiseOnTestCompletion(runningTest, TestExecutionResult.Failed);
        }

        /// <summary>
        /// Gets a collection containing all the environments that are used to execute a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <returns>A collection containing all the environments that are used to execute the test.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when there is no registered test with the given <paramref name="test"/> ID.
        /// </exception>
        public IEnumerable<IActiveEnvironment> EnvironmentsForTest(int test)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    m_Tests.ContainsKey(test),
                    Resources.Exceptions_Messages_NoTestWithTheGivenIDHasBeenRegistered);
            }

            var map = m_Tests[test];
            return map.Environments.Select(t => t.Item1).ToList();
        }

        /// <summary>
        /// Creates and returns the report for a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <returns>The report for the test.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when there is no registered test with the given <paramref name="test"/> ID.
        /// </exception>
        public IReport ReportFor(int test)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    m_Tests.ContainsKey(test),
                    Resources.Exceptions_Messages_NoTestWithTheGivenIDHasBeenRegistered);
            }

            var map = m_Tests[test];
            var builder = map.Reports;
            if (!builder.HasReportBeenFinalized)
            {
                builder.FinalizeReport();
            }

            return builder.Build();
        }

        /// <summary>
        /// Returns the collection of test complete notifications for the given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <returns>The collection of test complete notifications.</returns>
        public IEnumerable<TestCompletedNotification> NotificationsFor(int test)
        {
            {
                Lokad.Enforce.With<ArgumentException>(
                    m_Tests.ContainsKey(test),
                    Resources.Exceptions_Messages_NoTestWithTheGivenIDHasBeenRegistered);
            }

            var map = m_Tests[test];
            return map.Notifications;
        }

        /// <summary>
        /// An event raised when the test execution has completed.
        /// </summary>
        public event EventHandler<TestExecutionResultEventArgs> OnTestCompletion;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<int> GetEnumerator()
        {
            foreach (var key in m_Tests.Keys)
            {
                yield return key;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
