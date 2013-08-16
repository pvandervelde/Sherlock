//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for objects that implement <see cref="ITestExecutionNotifications"/>
    /// and need to provide external objects with access to their events.
    /// </summary>
    public interface ITestExecutionNotificationsInvoker : ITestExecutionNotifications
    {
        /// <summary>
        /// Raises the <see cref="ITestExecutionNotifications.OnExecutionProgress"/> event.
        /// </summary>
        /// <param name="sectionName">The name of the report section under which the results should be stored.</param>
        /// <param name="sectionReport">The report for the currently completed test section.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnExecutionProgress(string sectionName, TestSection sectionReport);

        /// <summary>
        /// Raises the <see cref="ITestExecutionNotifications.OnTestCompletion"/> event.
        /// </summary>
        /// <param name="testResult">The outcome of the test.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This method is used to raise an event, hence the naming.")]
        void RaiseOnTestCompletion(TestExecutionResult testResult);
    }
}
