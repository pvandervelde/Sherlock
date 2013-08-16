//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines the interface for objects that store active tests.
    /// </summary>
    internal interface IStoreActiveTests : IEnumerable<int>
    {
        /// <summary>
        /// Adds a test to the collection and associates it with a report builder.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="builder">The report builder that will generate the report for the test.</param>
        /// <param name="notifications">The collection of test complete notifications for the test.</param>
        void Add(int test, IReportBuilder builder, IEnumerable<TestCompletedNotification> notifications);

        /// <summary>
        /// Associates an environmentId with a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <param name="environment">The environmentId.</param>
        void AddEnvironmentForTest(int test, IActiveEnvironment environment);

        /// <summary>
        /// Removes the test with the given ID from the storage.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        void Remove(int test);

        /// <summary>
        /// Indicates that the specific environmentId has failed to return a response and is probably deceased.
        /// </summary>
        /// <param name="runningTest">The ID of the test that is running on the environment.</param>
        /// <param name="environmentId">The ID of the environment.</param>
        void EnvironmentFailure(int runningTest, string environmentId);

        /// <summary>
        /// Gets a collection containing all the environments that are used to execute a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <returns>A collection containing all the environments that are used to execute the test.</returns>
        IEnumerable<IActiveEnvironment> EnvironmentsForTest(int test);

        /// <summary>
        /// Creates and returns the report for a given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <returns>The report for the test.</returns>
        IReport ReportFor(int test);

        /// <summary>
        /// Returns the collection of test complete notifications for the given test.
        /// </summary>
        /// <param name="test">The ID of the test.</param>
        /// <returns>The collection of test complete notifications.</returns>
        IEnumerable<TestCompletedNotification> NotificationsFor(int test);

        /// <summary>
        /// An event raised when the test execution has completed.
        /// </summary>
        event EventHandler<TestExecutionResultEventArgs> OnTestCompletion;
    }
}
