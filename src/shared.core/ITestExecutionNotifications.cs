//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines the interface for notifications regarding test executions.
    /// </summary>
    public interface ITestExecutionNotifications : INotificationSet
    {
        /// <summary>
        /// An event raised when progress has been made in the test.
        /// </summary>
        event EventHandler<TestExecutionProgressEventArgs> OnExecutionProgress;

        /// <summary>
        /// An event raised when the test execution has completed.
        /// </summary>
        event EventHandler<TestExecutionResultEventArgs> OnTestCompletion;
    }
}
