//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines notification events that indicate progress and completion of a test.
    /// </summary>
    public sealed class TestExecutionNotifications : ITestExecutionNotificationsInvoker
    {
        /// <summary>
        /// An event raised when progress has been made in the test.
        /// </summary>
        public event EventHandler<TestExecutionProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// An event raised when the test execution has completed.
        /// </summary>
        public event EventHandler<TestExecutionResultEventArgs> OnTestCompletion;

        /// <summary>
        /// Raises the <see cref="ITestExecutionNotifications.OnExecutionProgress"/> event.
        /// </summary>
        /// <param name="sectionName">The name of the report section under which the results should be stored.</param>
        /// <param name="sectionReport">The report for the currently completed test section.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnExecutionProgress(string sectionName, TestSection sectionReport)
        {
            var local = OnExecutionProgress;
            if (local != null)
            {
                local(this, new TestExecutionProgressEventArgs(-1, sectionName, sectionReport));
            }
        }

        /// <summary>
        /// Raises the <see cref="ITestExecutionNotifications.OnTestCompletion"/> event.
        /// </summary>
        /// <param name="testResult">The outcome of the test.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        public void RaiseOnTestCompletion(TestExecutionResult testResult)
        {
            var local = OnTestCompletion;
            if (local != null)
            {
                local(this, new TestExecutionResultEventArgs(-1, testResult));
            }
        }
    }
}
