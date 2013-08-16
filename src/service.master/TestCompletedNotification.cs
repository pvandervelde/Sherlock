//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sherlock.Shared.Core;
using Sherlock.Shared.Core.Reporting;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Defines a base for classes that provide a method of notifying the test requestor that their test has completed.
    /// </summary>
    [Serializable]
    internal abstract class TestCompletedNotification
    {
        /// <summary>
        /// Sends out a notification indicating that the test has completed.
        /// </summary>
        /// <param name="result">The test result.</param>
        /// <param name="report">The report describing the results of the test.</param>
        public abstract void OnTestCompleted(TestExecutionResult result, IReport report);
    }
}
